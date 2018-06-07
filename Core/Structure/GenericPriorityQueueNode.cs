namespace Core.Structure
{
    public class GenericPriorityQueueNode<TPriority>
    {
        /// <summary>
        /// The Priority to insert this node at.  Must be set BEFORE adding a node to the queue (ideally just once, in the node's constructor).
        /// Should not be manually edited once the node has been enqueued - use queue.UpdatePriority() instead
        /// </summary>
        public TPriority priority { get; protected internal set; }

        /// <summary>
        /// Represents the current position in the queue
        /// </summary>
        public int queueIndex { get; internal set; }

        /// <summary>
        /// Represents the order the node was inserted in
        /// </summary>
        public long insertionIndex { get; internal set; }
    }
}
