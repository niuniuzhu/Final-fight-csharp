namespace Core.Structure
{
    public class StablePriorityQueueNode : FastPriorityQueueNode
    {
        /// <summary>
        /// Represents the order the node was inserted in
        /// </summary>
        public long insertionIndex { get; internal set; }
    }
}
