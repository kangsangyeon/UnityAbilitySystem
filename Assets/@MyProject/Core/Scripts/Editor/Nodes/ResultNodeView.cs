using Core.Nodes;

namespace Core.Editor.Nodes
{
    [NodeType(typeof(ResultNode))]
    public class ResultNodeView : NodeView
    {
        public ResultNodeView()
        {
            // ResultNode 인스턴스는 NodeGraph를 처음 열어볼 때 NodeGraphView에서 생성합니다.
            title = "Result";
            inputs.Add(CreateInputPort());
        }
    }
}