using KeyValuePair = Opc.Ua.KeyValuePair;
using TypeInfo = Opc.Ua.TypeInfo;

namespace Station.Domain.Accessors.Managers;
internal sealed class NodeManager : CustomNodeManager2
{
    public NodeManager(IServerInternal server, ApplicationConfiguration configuration) : base(server, configuration, Sign.Namespace) { }
    public override NodeId New(ISystemContext context, NodeState node)
    {//重寫NodeId生成方式(目前採用'_'分隔,如需更改,請修改此方法)
        if (node is BaseInstanceState instance && instance.Parent is not null)
        {
            if (instance.Parent.NodeId.Identifier is string id)
            {
                return new NodeId(id + "_" + instance.SymbolicName, instance.Parent.NodeId.NamespaceIndex);
            }
        }
        return node.NodeId;
    }
    protected override NodeHandle? GetManagerHandle(ServerSystemContext context, NodeId nodeId, IDictionary<NodeId, NodeState> cache)
    {//重寫獲取節點句柄的方法
        lock (Lock)
        {
            //快速排除不在命名空間中的節點。
            if (!IsNodeIdInNamespace(nodeId)) return null;
            if (!PredefinedNodes.TryGetValue(nodeId, out NodeState? node)) return null;
            return new NodeHandle()
            {
                NodeId = nodeId,
                Node = node,
                Validated = true
            };
        }
    }
    protected override NodeState? ValidateNode(ServerSystemContext context, NodeHandle handle, IDictionary<NodeId, NodeState> cache)
    {//重寫節點的驗證方式

        //如果沒有根則無效。
        if (handle == null) return null;

        //檢查之前是否經過驗證。
        if (handle.Validated) return handle.Node;

        // TBD
        return null;
    }
    public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
    {//重寫創建基礎目錄
        lock (Lock)
        {
            if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out _references))
            {
                externalReferences[ObjectIds.ObjectsFolder] = _references = new List<IReference>();
            }
            try
            {
                GeneraterNodes(new List<PathNode>()
                {
                    new()
                    {
                        NodeId = 1,
                        IsTerminal = false,
                        NodeName = "Components",
                        NodePath = "1",
                        NodeType = NodeType.Scada,
                        ParentPath = ""
                    },
                    new()
                    {
                        NodeId = 11,
                        IsTerminal = false,
                        NodeName = "子目錄-1",
                        NodePath = "11",
                        NodeType = NodeType.Channel,
                        ParentPath = "1"
                    },
                    new()
                    {
                        NodeId = 12,
                        IsTerminal = false,
                        NodeName = "子目錄-1",
                        NodePath = "12",
                        NodeType = NodeType.Device,
                        ParentPath = "1"
                    },
                    new()
                    {
                        NodeId = 111,
                        IsTerminal = true,
                        NodeName = "葉子節點-1",
                        NodePath = "111",
                        NodeType = NodeType.Measure,
                        ParentPath = "11"
                    },
                    new()
                    {
                        NodeId = 112,
                        IsTerminal = true,
                        NodeName = "葉子節點-2",
                        NodePath = "112",
                        NodeType = NodeType.Measure,
                        ParentPath = "11"
                    },
                    new()
                    {
                        NodeId = 113,
                        IsTerminal = true,
                        NodeName = "葉子節點-3",
                        NodePath = "113",
                        NodeType = NodeType.Measure,
                        ParentPath = "11"
                    },
                    new()
                    {
                        NodeId = 114,
                        IsTerminal = true,
                        NodeName = "葉子節點-4",
                        NodePath = "114",
                        NodeType = NodeType.Measure,
                        ParentPath = "11"
                    },
                    new()
                    {
                        NodeId = 121,
                        IsTerminal = true,
                        NodeName = "葉子節點-1",
                        NodePath = "121",
                        NodeType = NodeType.Measure,
                        ParentPath = "12"
                    },
                    new()
                    {
                        NodeId = 122,
                        IsTerminal = true,
                        NodeName = "葉子節點-2",
                        NodePath = "122",
                        NodeType = NodeType.Measure,
                        ParentPath = "12"
                    }
                }, _references);

                //實時更新測點的數據
                UpdateVariableValue();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("調用接口初始化觸發異常:" + ex.Message);
                Console.ResetColor();
            }
        }
    }
    void GeneraterNodes(IEnumerable<PathNode> nodes, IList<IReference> references)
    {//生成根節點(由於根節點需要特殊處理,此處單獨出來一個方法)
        foreach (var node in nodes.Where(item => item.NodeType == NodeType.Scada))
        {
            try
            {
                var root = CreateFolder(parent: default, node.NodePath, node.NodeName);
                root.AddReference(ReferenceTypes.Organizes, isInverse: true, ObjectIds.ObjectsFolder);
                references.Add(new NodeStateReference(ReferenceTypes.Organizes, isInverse: false, root.NodeId));
                root.EventNotifier = EventNotifiers.SubscribeToEvents;
                AddRootNotifier(root);
                CreateNodes(nodes, root, node.NodePath);
                FolderDic.Add(node.NodePath, root);

                //添加引用關係
                AddPredefinedNode(SystemContext, root);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("創建 OPC UA 根節點觸發異常:" + ex.Message);
                Console.ResetColor();
            }
        }
    }
    void CreateNodes(IEnumerable<PathNode> nodes, FolderState parent, string parentPath)
    {//遞歸創建子節點(包括創建目錄和測點)
        foreach (var node in nodes.Where(item => item.ParentPath == parentPath))
        {
            try
            {
                if (!node.IsTerminal)
                {
                    var folder = CreateFolder(parent, node.NodePath, node.NodeName);
                    FolderDic.Add(node.NodePath, folder);
                    CreateNodes(nodes, folder, node.NodePath);
                }
                else
                {
                    var variable = CreateVariable(parent, node.NodePath, node.NodeName, DataTypeIds.Double, ValueRanks.Scalar);

                    //此處需要注意  目錄字典是以目錄路徑作為KEY 而 測點字典是以測點ID作為KEY  為了方便更新實時數據
                    NodeDic.Add(node.NodeId.ToString(), variable);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("創建 OPC UA 子節點觸發異常:" + ex.Message);
                Console.ResetColor();
            }
        }
    }
    FolderState CreateFolder(NodeState? parent, string path, string name)
    {//創建目錄
        FolderState folder = new(parent)
        {
            SymbolicName = name,
            ReferenceTypeId = ReferenceTypes.Organizes,
            TypeDefinitionId = ObjectTypeIds.FolderType,
            NodeId = new NodeId(path, NamespaceIndex),
            BrowseName = new QualifiedName(path, NamespaceIndex),
            DisplayName = new LocalizedText("en", name),
            WriteMask = AttributeWriteMask.None,
            UserWriteMask = AttributeWriteMask.None,
            EventNotifier = EventNotifiers.None
        };
        parent?.AddChild(folder);
        return folder;
    }
    BaseDataVariableState CreateVariable(NodeState parent, string path, string name, NodeId dataType, int valueRank)
    {//創建節點
        BaseDataVariableState variable = new(parent)
        {
            SymbolicName = name,
            ReferenceTypeId = ReferenceTypes.Organizes,
            TypeDefinitionId = VariableTypeIds.BaseDataVariableType,
            NodeId = new NodeId(path, NamespaceIndex),
            BrowseName = new QualifiedName(path, NamespaceIndex),
            DisplayName = new LocalizedText("en", name),
            WriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description,
            UserWriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description,
            DataType = dataType,
            ValueRank = valueRank,
            AccessLevel = AccessLevels.CurrentReadOrWrite,
            UserAccessLevel = AccessLevels.CurrentReadOrWrite,
            Historizing = false,
            //variable.Value = GetNewValue(variable);
            StatusCode = StatusCodes.Good,
            Timestamp = DateTime.UtcNow,
            OnWriteValue = OnWriteDataValue,
            OnReadValue = OnReadDataValue
        };
        if (valueRank == ValueRanks.OneDimension)
        {
            variable.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0 });
        }
        else if (valueRank == ValueRanks.TwoDimensions)
        {
            variable.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0, 0 });
        }
        parent?.AddChild(variable);
        return variable;
    }
    ServiceResult OnReadDataValue(ISystemContext context, NodeState node, NumericRange indexRange, QualifiedName dataEncoding, ref object value, ref StatusCode statusCode, ref DateTime timestamp)
    {
        return ServiceResult.Good;
    }
    public void UpdateVariableValue()
    {//實時更新節點數據
        Task.Run(() =>
        {
            while (true)
            {
                try
                {
                    /*
                     * 此處僅作示例代碼  所以不修改節點樹 故將UpdateNodesAttribute()方法跳過
                     * 在實際業務中  請根據自身的業務需求決定何時修改節點菜單樹
                     */
                    int count = 0;

                    //配置發生更改時,重新生成節點樹
                    if (count > 0 && count != FgCount)
                    {
                        FgCount = count;
                        List<PathNode> nodes = new();
                        /*
                         * 此處有想過刪除整個菜單樹,然後重建 保證各個NodeId仍與原來的一直
                         * 但是 後來發現這樣會導致原來的客戶端訂閱信息丟失  無法獲取訂閱數據
                         * 所以  只能一級級的檢查節點  然後修改屬性
                         */
                        UpdateNodesAttribute(nodes);
                    }

                    //模擬獲取實時數據
                    BaseDataVariableState? node = null;

                    /*
                     * 在實際業務中應該是根據對應的標識來更新固定節點的數據
                     * 這裡  我偷個懶  全部測點都更新為一個新的隨機數
                     */
                    foreach (var item in NodeDic)
                    {
                        node = item.Value;
                        node.Value = 666; //RandomLibrary.GetRandomInt(0, 99)
                        node.Timestamp = DateTime.UtcNow;

                        //變更標識  只有執行了這一步,訂閱的客戶端才會收到新的數據
                        node.ClearChangeMasks(SystemContext, includeChildren: false);
                    }

                    //1秒更新一次
                    Thread.Sleep(1000 * 1);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("更新 OPC UA 節點數據觸發異常:" + ex.Message);
                    Console.ResetColor();
                }
            }
        });
    }
    public void UpdateNodesAttribute(List<PathNode> nodes)
    {//修改節點樹(添加節點,刪除節點,修改節點名稱)
        //修改或創建根節點
        foreach (var node in nodes.Where(item => item.NodeType == NodeType.Scada))
        {
            if (!FolderDic.TryGetValue(node.NodePath, out FolderState? scadaNode))
            {
                //如果根節點都不存在  那麼整個樹都需要創建
                var root = CreateFolder(parent: null, node.NodePath, node.NodeName);
                root.AddReference(ReferenceTypes.Organizes, isInverse: true, ObjectIds.ObjectsFolder);
                _references?.Add(new NodeStateReference(ReferenceTypes.Organizes, isInverse: false, root.NodeId));
                root.EventNotifier = EventNotifiers.SubscribeToEvents;
                AddRootNotifier(root);
                CreateNodes(nodes, root, node.NodePath);
                FolderDic.Add(node.NodePath, root);
                AddPredefinedNode(SystemContext, root);
                continue;
            }
            scadaNode.DisplayName = node.NodeName;
            scadaNode.ClearChangeMasks(SystemContext, includeChildren: false);
        }

        //修改或創建目錄(此處設計為可以有多級目錄,上面是演示數據,所以我只寫了三級,事實上更多級也是可以的)
        foreach (var node in nodes.Where(item => item.NodeType != NodeType.Scada && !item.IsTerminal))
        {
            if (!FolderDic.TryGetValue(node.NodePath, out FolderState? folder))
            {
                var par = GetParentFolderState(nodes, node);
                folder = CreateFolder(par, node.NodePath, node.NodeName);
                AddPredefinedNode(SystemContext, folder);
                par?.ClearChangeMasks(SystemContext, includeChildren: false);
                FolderDic.Add(node.NodePath, folder);
            }
            else
            {
                folder.DisplayName = node.NodeName;
                folder.ClearChangeMasks(SystemContext, includeChildren: false);
            }
        }

        //修改或創建測點
        //這裡我的數據結構採用IsTerminal來代表是否是測點  實際業務中可能需要根據自身需要調整
        foreach (var node in nodes.Where(item => item.IsTerminal))
        {
            if (NodeDic.TryGetValue(node.NodeId.ToString(), out BaseDataVariableState? variableState))
            {
                variableState.DisplayName = node.NodeName;
                variableState.Timestamp = DateTime.UtcNow;
                variableState.ClearChangeMasks(SystemContext, includeChildren: false);
            }
            else
            {
                if (FolderDic.TryGetValue(node.ParentPath, out FolderState? folderState))
                {
                    variableState = CreateVariable(folderState, node.NodePath, node.NodeName, DataTypeIds.Double, ValueRanks.Scalar);
                    AddPredefinedNode(SystemContext, variableState);
                    folderState.ClearChangeMasks(SystemContext, includeChildren: false);
                    NodeDic.Add(node.NodeId.ToString(), variableState);
                }
            }
        }

        /*
         * 將新獲取到的菜單列表與原列表對比
         * 如果新菜單列表中不包含原有的菜單  
         * 則說明這個菜單被刪除了  這裡也需要刪除
         */
        List<string> folderPaths = FolderDic.Keys.ToList();
        var remNode = NodeDic.Keys.ToArray().Except(nodes.Where(item => item.IsTerminal).Select(item => item.NodeId.ToString()));
        foreach (var str in remNode)
        {
            if (NodeDic.TryGetValue(str, out BaseDataVariableState? node))
            {
                var parent = node.Parent;
                parent.RemoveChild(node);
                NodeDic.Remove(str);
            }
        }
        foreach (var folderPath in folderPaths.Except(nodes.Where(item => !item.IsTerminal).Select(item => item.NodePath)))
        {
            if (FolderDic.TryGetValue(folderPath, out FolderState? folder))
            {
                var parent = folder.Parent;
                if (parent is not null)
                {
                    parent.RemoveChild(folder);
                    FolderDic.Remove(folderPath);
                }
                else
                {
                    RemoveRootNotifier(folder);
                    RemovePredefinedNode(SystemContext, folder, new List<LocalReference>());
                }
            }
        }
    }
    public FolderState? GetParentFolderState(IEnumerable<PathNode> nodes, PathNode currentNode)
    {//創建父級目錄(請確保對應的根目錄已創建)
        if (!FolderDic.TryGetValue(currentNode.ParentPath, out FolderState? folder))
        {
            var parent = nodes.FirstOrDefault(d => d.NodePath == currentNode.ParentPath);
            if (!string.IsNullOrEmpty(parent.ParentPath))
            {
                var pFol = GetParentFolderState(nodes, parent);
                folder = CreateFolder(pFol, parent.NodePath, parent.NodeName);
                pFol?.ClearChangeMasks(SystemContext, includeChildren: false);
                AddPredefinedNode(SystemContext, folder);
                FolderDic.Add(currentNode.ParentPath, folder);
            }
        }
        return folder;
    }
    ServiceResult OnWriteDataValue(ISystemContext context, NodeState node, NumericRange indexRange, QualifiedName dataEncoding, ref object value, ref StatusCode statusCode, ref DateTime timestamp)
    {//客戶端寫入值時觸發(綁定到節點的寫入事件上)
        var variable = node as BaseDataVariableState;
        try
        {
            //驗證數據類型
            var typeInfo = TypeInfo.IsInstanceOfDataType(value, variable?.DataType, variable.ValueRank, context.NamespaceUris, context.TypeTable);
            if (typeInfo is null || typeInfo == TypeInfo.Unknown) return StatusCodes.BadTypeMismatch;
            if (typeInfo.BuiltInType == BuiltInType.Double)
            {
                var number = Convert.ToDouble(value);
                value = TypeInfo.Cast(number, typeInfo.BuiltInType);
            }
            return ServiceResult.Good;
        }
        catch (Exception)
        {
            return StatusCodes.BadTypeMismatch;
        }
    }
    public override void HistoryRead(OperationContext context, HistoryReadDetails details, TimestampsToReturn timestampsToReturn, bool releaseContinuationPoints,
        IList<HistoryReadValueId> nodesToRead, IList<HistoryReadResult> results, IList<ServiceResult> errors)
    {//讀取歷史數據
        //假設查詢歷史數據都是帶上時間範圍的
        if (details is not ReadProcessedDetails readDetail || readDetail.StartTime == DateTime.MinValue || readDetail.EndTime == DateTime.MinValue)
        {
            errors[0] = StatusCodes.BadHistoryOperationUnsupported;
            return;
        }
        for (int ii = 0; ii < nodesToRead.Count; ii++)
        {
            int sss = readDetail.StartTime.Millisecond;
            var res = sss + DateTime.UtcNow.Millisecond;

            //這裡  返回的歷史數據可以是多種數據類型  請根據實際的業務來選擇
            KeyValuePair keyValue = new()
            {
                Key = new QualifiedName(nodesToRead[ii].NodeId.Identifier.ToString()),
                Value = res
            };
            results[ii] = new HistoryReadResult()
            {
                StatusCode = StatusCodes.Good,
                HistoryData = new ExtensionObject(keyValue)
            };
            errors[ii] = StatusCodes.Good;

            //切記,如果你已處理完了讀取歷史數據的操作,請將Processed設為true,這樣 OPC UA 類庫就知道你已經處理過了 不需要再進行檢查了
            nodesToRead[ii].Processed = true;
        }
    }
    public enum NodeType
    {
        /// <summary>
        /// 根節點
        /// </summary>
        Scada = 1,

        /// <summary>
        /// 目錄
        /// </summary>
        Channel = 2,

        /// <summary>
        /// 目錄
        /// </summary>
        Device = 3,

        /// <summary>
        /// 測點
        /// </summary>
        Measure = 4
    }
    public readonly record struct PathNode
    {
        /// <summary>
        /// 節點路徑(逐級拼接)
        /// </summary>
        public required string NodePath { get; init; }

        /// <summary>
        /// 父節點路徑(逐級拼接)
        /// </summary>
        public required string ParentPath { get; init; }

        /// <summary>
        /// 節點編號 (在我的業務系統中的節點編號並不完全唯一,但是所有測點Id都是不同的)
        /// </summary>
        public required int NodeId { get; init; }

        /// <summary>
        /// 節點名稱(展示名稱)
        /// </summary>
        public required string NodeName { get; init; }

        /// <summary>
        /// 是否端點(最底端子節點)
        /// </summary>
        public required bool IsTerminal { get; init; }

        /// <summary>
        /// 節點類型
        /// </summary>
        public required NodeType NodeType { get; init; }
    }

    //測點集合,實時數據刷新時,直接從字典中取出對應的測點,修改值即可
    Dictionary<string, BaseDataVariableState> NodeDic { get; set; } = new();

    //目錄集合,修改菜單樹時需要(我們需要知道哪些菜單需要修改,哪些需要新增,哪些需要刪除)
    Dictionary<string, FolderState> FolderDic { get; set; } = new();

    //配置修改次數  主要用來識別菜單樹是否有變動  如果發生變動則修改菜單樹對應節點  測點的實時數據變化不算在內
    int FgCount { get; set; } = -1;
    IList<IReference>? _references;
}