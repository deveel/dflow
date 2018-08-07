﻿using System;
using System.Threading.Tasks;

namespace Deveel.Workflows {
    public class SubProcess : Activity, ISequenceHandler {
        public SubProcess(string id) : base(id) {
            Sequence = new ProcessSequence(this);
        }

        public override FlowNodeType NodeType => FlowNodeType.Process;

        public ProcessSequence Sequence { get; }

        ISequenceHandler ISequenceHandler.Parent { get; set; }

        protected override async Task ExecuteActivityAsync(ActivityContext context, object state) {
            foreach (var node in Sequence) {
                await node.ExecuteAsync(context);
            }
        }

        void ISequenceHandler.OnNodeAttached(FlowNode node) {
            OnNodeAttached(node);
        }

        void ISequenceHandler.OnNodeDetached(FlowNode node) {
            OnNodeDetached(node);
        }

        bool ISequenceHandler.NodeExists(FlowNode node) {
            return Sequence.Contains(node.Id);
        }

        protected virtual void OnNodeAttached(FlowNode node) {

        }

        protected virtual void OnNodeDetached(FlowNode node) {

        }
    }
}