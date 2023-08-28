using System;
using UnityEngine;

namespace Core.Nodes
{
    public abstract class CodeFunctionNode : AbstractNode, ICloneable
    {
        // TODO: value property의 사용을 중단하고, 전부 CalculateValue()를 사용하도록 대체했습니다.
        // 이는 하나의 graph scriptable object를 여러 상황에서 재사용될 수 있어야 하는데,
        // graph 내 여러 노드들(ex: stat node, level node 등)이 어느 특정한 entity가 소유한 객체를 레퍼런스로 가지고 있을 수 있기 때문에 문제가 있습니다.
        // 따라서 entity가 소유한 객체를 매번 쿼리해 계산하는 함수인 CalculateValue()를 우선 사용하도록 합니다.
        public abstract float value { get; }

        public abstract float CalculateValue(GameObject _source);
        public abstract object Clone();
    }
}