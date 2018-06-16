using Core.Math;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.Structure
{
	/// <summary>
	/// A copy of FastPriorityQueue which is also stable - that is, when two nodes are enqueued with the same priority, they
	/// are always dequeued in the same order.
	/// See https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp/wiki/Getting-Started for more information
	/// </summary>
	/// <typeparam name="T">The values in the queue.  Must extend the StablePriorityQueueNode class</typeparam>
	public sealed class StablePriorityQueue<T> : IFixedSizePriorityQueue<T, float>
        where T : StablePriorityQueueNode
    {
        private int _numNodes;
        private T[] _nodes;
        private long _numNodesEverEnqueued;

        /// <summary>
        /// Instantiate a new Priority Queue
        /// </summary>
        /// <param name="maxNodes">The max nodes ever allowed to be enqueued (going over this will cause undefined behavior)</param>
        public StablePriorityQueue(int maxNodes)
        {
            #if DEBUG
            if (maxNodes <= 0)
            {
                throw new InvalidOperationException("New queue size cannot be smaller than 1");
            }
            #endif

            this._numNodes = 0;
            this._nodes = new T[maxNodes + 1];
            this._numNodesEverEnqueued = 0;
        }

        /// <summary>
        /// Returns the number of nodes in the queue.
        /// O(1)
        /// </summary>
        public int count
        {
            get
            {
                return this._numNodes;
            }
        }

        /// <summary>
        /// Returns the maximum number of items that can be enqueued at once in this queue.  Once you hit this number (ie. once Count == MaxSize),
        /// attempting to enqueue another item will cause undefined behavior.  O(1)
        /// </summary>
        public int maxSize
        {
            get
            {
                return this._nodes.Length - 1;
            }
        }

        /// <summary>
        /// Removes every node from the queue.
        /// O(n) (So, don't do this often!)
        /// </summary>
        #if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        #endif
        public void Clear()
        {
            Array.Clear(this._nodes, 1, this._numNodes);
            this._numNodes = 0;
        }

        /// <summary>
        /// Returns (in O(1)!) whether the given node is in the queue.  O(1)
        /// </summary>
        #if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        #endif
        public bool Contains(T node)
        {
            #if DEBUG
            if(node == null)
            {
                throw new ArgumentNullException("node");
            }
            if(node.queueIndex < 0 || node.queueIndex >= this._nodes.Length)
            {
                throw new InvalidOperationException("node.QueueIndex has been corrupted. Did you change it manually? Or add this node to another queue?");
            }
            #endif

            return (this._nodes[node.queueIndex] == node);
        }

        /// <summary>
        /// Enqueue a node to the priority queue.  Lower values are placed in front. Ties are broken by first-in-first-out.
        /// If the queue is full, the result is undefined.
        /// If the node is already enqueued, the result is undefined.
        /// O(log n)
        /// </summary>
        #if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        #endif
        public void Enqueue(T node, float priority)
        {
            #if DEBUG
            if(node == null)
            {
                throw new ArgumentNullException("node");
            }
            if(this._numNodes >= this._nodes.Length - 1)
            {
                throw new InvalidOperationException("Queue is full - node cannot be added: " + node);
            }
            if(this.Contains(node))
            {
                throw new InvalidOperationException("Node is already enqueued: " + node);
            }
            #endif

            node.priority = priority;
            this._numNodes++;
            this._nodes[this._numNodes] = node;
            node.queueIndex = this._numNodes;
            node.insertionIndex = this._numNodesEverEnqueued++;
            this.CascadeUp(node);
        }

        //Performance appears to be slightly better when this is NOT inlined o_O
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void CascadeUp(T node)
        {
            //aka Heapify-up
            int parent;
            if(node.queueIndex > 1)
            {
                parent = node.queueIndex >> 1;
                T parentNode = this._nodes[parent];
                if(this.HasHigherPriority(parentNode, node))
                    return;

                //Node has lower priority value, so move parent down the heap to make room
                this._nodes[node.queueIndex] = parentNode;
                parentNode.queueIndex = node.queueIndex;

                node.queueIndex = parent;
            }
            else
            {
                return;
            }
            while(parent > 1)
            {
                parent >>= 1;
                T parentNode = this._nodes[parent];
                if(this.HasHigherPriority(parentNode, node))
                    break;

                //Node has lower priority value, so move parent down the heap to make room
                this._nodes[node.queueIndex] = parentNode;
                parentNode.queueIndex = node.queueIndex;

                node.queueIndex = parent;
            }
            this._nodes[node.queueIndex] = node;
        }

#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void CascadeDown(T node)
        {
            //aka Heapify-down
            int finalQueueIndex = node.queueIndex;
            int childLeftIndex = 2 * finalQueueIndex;

            // If leaf node, we're done
            if(childLeftIndex > this._numNodes)
            {
                return;
            }

            // Check if the left-child is higher-priority than the current node
            int childRightIndex = childLeftIndex + 1;
            T childLeft = this._nodes[childLeftIndex];
            if(this.HasHigherPriority(childLeft, node))
            {
                // Check if there is a right child. If not, swap and finish.
                if(childRightIndex > this._numNodes)
                {
                    node.queueIndex = childLeftIndex;
                    childLeft.queueIndex = finalQueueIndex;
                    this._nodes[finalQueueIndex] = childLeft;
                    this._nodes[childLeftIndex] = node;
                    return;
                }
                // Check if the left-child is higher-priority than the right-child
                T childRight = this._nodes[childRightIndex];
                if(this.HasHigherPriority(childLeft, childRight))
                {
                    // left is highest, move it up and continue
                    childLeft.queueIndex = finalQueueIndex;
                    this._nodes[finalQueueIndex] = childLeft;
                    finalQueueIndex = childLeftIndex;
                }
                else
                {
                    // right is even higher, move it up and continue
                    childRight.queueIndex = finalQueueIndex;
                    this._nodes[finalQueueIndex] = childRight;
                    finalQueueIndex = childRightIndex;
                }
            }
            // Not swapping with left-child, does right-child exist?
            else if(childRightIndex > this._numNodes)
            {
                return;
            }
            else
            {
                // Check if the right-child is higher-priority than the current node
                T childRight = this._nodes[childRightIndex];
                if(this.HasHigherPriority(childRight, node))
                {
                    childRight.queueIndex = finalQueueIndex;
                    this._nodes[finalQueueIndex] = childRight;
                    finalQueueIndex = childRightIndex;
                }
                // Neither child is higher-priority than current, so finish and stop.
                else
                {
                    return;
                }
            }

            while(true)
            {
                childLeftIndex = 2 * finalQueueIndex;

                // If leaf node, we're done
                if(childLeftIndex > this._numNodes)
                {
                    node.queueIndex = finalQueueIndex;
                    this._nodes[finalQueueIndex] = node;
                    break;
                }

                // Check if the left-child is higher-priority than the current node
                childRightIndex = childLeftIndex + 1;
                childLeft = this._nodes[childLeftIndex];
                if(this.HasHigherPriority(childLeft, node))
                {
                    // Check if there is a right child. If not, swap and finish.
                    if(childRightIndex > this._numNodes)
                    {
                        node.queueIndex = childLeftIndex;
                        childLeft.queueIndex = finalQueueIndex;
                        this._nodes[finalQueueIndex] = childLeft;
                        this._nodes[childLeftIndex] = node;
                        break;
                    }
                    // Check if the left-child is higher-priority than the right-child
                    T childRight = this._nodes[childRightIndex];
                    if(this.HasHigherPriority(childLeft, childRight))
                    {
                        // left is highest, move it up and continue
                        childLeft.queueIndex = finalQueueIndex;
                        this._nodes[finalQueueIndex] = childLeft;
                        finalQueueIndex = childLeftIndex;
                    }
                    else
                    {
                        // right is even higher, move it up and continue
                        childRight.queueIndex = finalQueueIndex;
                        this._nodes[finalQueueIndex] = childRight;
                        finalQueueIndex = childRightIndex;
                    }
                }
                // Not swapping with left-child, does right-child exist?
                else if(childRightIndex > this._numNodes)
                {
                    node.queueIndex = finalQueueIndex;
                    this._nodes[finalQueueIndex] = node;
                    break;
                }
                else
                {
                    // Check if the right-child is higher-priority than the current node
                    T childRight = this._nodes[childRightIndex];
                    if(this.HasHigherPriority(childRight, node))
                    {
                        childRight.queueIndex = finalQueueIndex;
                        this._nodes[finalQueueIndex] = childRight;
                        finalQueueIndex = childRightIndex;
                    }
                    // Neither child is higher-priority than current, so finish and stop.
                    else
                    {
                        node.queueIndex = finalQueueIndex;
                        this._nodes[finalQueueIndex] = node;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if 'higher' has higher priority than 'lower', false otherwise.
        /// Note that calling HasHigherPriority(node, node) (ie. both arguments the same node) will return false
        /// </summary>
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        #endif
        private bool HasHigherPriority(T higher, T lower)
        {
            return (higher.priority < lower.priority ||
                (higher.priority == lower.priority && higher.insertionIndex < lower.insertionIndex));
        }

        /// <summary>
        /// Removes the head of the queue (node with minimum priority; ties are broken by order of insertion), and returns it.
        /// If queue is empty, result is undefined
        /// O(log n)
        /// </summary>
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public T Dequeue()
        {
            #if DEBUG
            if(this._numNodes <= 0)
            {
                throw new InvalidOperationException("Cannot call Dequeue() on an empty queue");
            }

            if(!this.IsValidQueue())
            {
                throw new InvalidOperationException("Queue has been corrupted (Did you update a node priority manually instead of calling UpdatePriority()?" +
                                                    "Or add the same node to two different queues?)");
            }
            #endif

            T returnMe = this._nodes[1];
            //If the node is already the last node, we can remove it immediately
            if(this._numNodes == 1)
            {
                this._nodes[1] = null;
                this._numNodes = 0;
                return returnMe;
            }

            //Swap the node with the last node
            T formerLastNode = this._nodes[this._numNodes];
            this._nodes[1] = formerLastNode;
            formerLastNode.queueIndex = 1;
            this._nodes[this._numNodes] = null;
            this._numNodes--;

            //Now bubble formerLastNode (which is no longer the last node) down
            this.CascadeDown(formerLastNode);
            return returnMe;
        }

        /// <summary>
        /// Resize the queue so it can accept more nodes.  All currently enqueued nodes are remain.
        /// Attempting to decrease the queue size to a size too small to hold the existing nodes results in undefined behavior
        /// O(n)
        /// </summary>
        public void Resize(int maxNodes)
        {
            #if DEBUG
            if (maxNodes <= 0)
            {
                throw new InvalidOperationException("Queue size cannot be smaller than 1");
            }

            if (maxNodes < this._numNodes)
            {
                throw new InvalidOperationException("Called Resize(" + maxNodes + "), but current queue contains " + this._numNodes + " nodes");
            }
            #endif

            T[] newArray = new T[maxNodes + 1];
            int highestIndexToCopy = MathUtils.Min(maxNodes, this._numNodes);
            Array.Copy(this._nodes, newArray, highestIndexToCopy + 1);
            this._nodes = newArray;
        }

        /// <summary>
        /// Returns the head of the queue, without removing it (use Dequeue() for that).
        /// If the queue is empty, behavior is undefined.
        /// O(1)
        /// </summary>
        public T first
        {
            get
            {
                #if DEBUG
                if(this._numNodes <= 0)
                {
                    throw new InvalidOperationException("Cannot call .First on an empty queue");
                }
                #endif

                return this._nodes[1];
            }
        }

        /// <summary>
        /// This method must be called on a node every time its priority changes while it is in the queue.  
        /// <b>Forgetting to call this method will result in a corrupted queue!</b>
        /// Calling this method on a node not in the queue results in undefined behavior
        /// O(log n)
        /// </summary>
        #if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        #endif
        public void UpdatePriority(T node, float priority)
        {
            #if DEBUG
            if(node == null)
            {
                throw new ArgumentNullException("node");
            }
            if(!this.Contains(node))
            {
                throw new InvalidOperationException("Cannot call UpdatePriority() on a node which is not enqueued: " + node);
            }
            #endif

            node.priority = priority;
            this.OnNodeUpdated(node);
        }

#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private void OnNodeUpdated(T node)
        {
            //Bubble the updated node up or down as appropriate
            int parentIndex = node.queueIndex >> 1;

            if(parentIndex > 0 && this.HasHigherPriority(node, this._nodes[parentIndex]))
            {
                this.CascadeUp(node);
            }
            else
            {
                //Note that CascadeDown will be called if parentNode == node (that is, node is the root)
                this.CascadeDown(node);
            }
        }

        /// <summary>
        /// Removes a node from the queue.  The node does not need to be the head of the queue.  
        /// If the node is not in the queue, the result is undefined.  If unsure, check Contains() first
        /// O(log n)
        /// </summary>
#if NET_VERSION_4_5
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Remove(T node)
        {
#if DEBUG
            if(node == null)
            {
                throw new ArgumentNullException("node");
            }
            if(!this.Contains(node))
            {
                throw new InvalidOperationException("Cannot call Remove() on a node which is not enqueued: " + node);
            }
#endif

            //If the node is already the last node, we can remove it immediately
            if(node.queueIndex == this._numNodes)
            {
                this._nodes[this._numNodes] = null;
                this._numNodes--;
                return;
            }

            //Swap the node with the last node
            T formerLastNode = this._nodes[this._numNodes];
            this._nodes[node.queueIndex] = formerLastNode;
            formerLastNode.queueIndex = node.queueIndex;
            this._nodes[this._numNodes] = null;
            this._numNodes--;

            //Now bubble formerLastNode (which is no longer the last node) up or down as appropriate
            this.OnNodeUpdated(formerLastNode);
        }

        public IEnumerator<T> GetEnumerator()
        {
#if NET_VERSION_4_5 // ArraySegment does not implement IEnumerable before 4.5
            IEnumerable<T> e = new ArraySegment<T>(_nodes, 1, _numNodes);
            return e.GetEnumerator();
#else
            for(int i = 1; i <= this._numNodes; i++)
                yield return this._nodes[i];
#endif
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// <b>Should not be called in production code.</b>
        /// Checks to make sure the queue is still in a valid state.  Used for testing/debugging the queue.
        /// </summary>
        public bool IsValidQueue()
        {
            for(int i = 1; i < this._nodes.Length; i++)
            {
                if(this._nodes[i] != null)
                {
                    int childLeftIndex = 2 * i;
                    if(childLeftIndex < this._nodes.Length && this._nodes[childLeftIndex] != null && this.HasHigherPriority(this._nodes[childLeftIndex], this._nodes[i]))
                        return false;

                    int childRightIndex = childLeftIndex + 1;
                    if(childRightIndex < this._nodes.Length && this._nodes[childRightIndex] != null && this.HasHigherPriority(this._nodes[childRightIndex], this._nodes[i]))
                        return false;
                }
            }
            return true;
        }
    }
}