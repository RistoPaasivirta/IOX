/// <summary>
/// Non-array based list where each node holds a pointer to previous and next node
/// </summary>
public class DoubleLinkedList<T>
{
    public class ListNode
    {
        public T Data { get; private set; }
        public ListNode Previous { get; set; }
        public ListNode Next { get; set; }

        public ListNode(T dataValue) : this(dataValue, null, null) { }
        public ListNode(T dataValue, ListNode nextNode, ListNode previousNode)
        {
            Data = dataValue;
            Previous = previousNode;
            Next = nextNode;
        }
    }

    public int Count { get; private set; }
    public bool Empty { get { return Count == 0; } }
    public ListNode Head { get; private set; }
    public ListNode Tail { get; private set; }

    public DoubleLinkedList() { Head = Tail = null; }

    public void InsertFront(T item)
    {
        if (Empty)
            Head = Tail = new ListNode(item);
        else
            Head.Previous = Head = new ListNode(item, Head, null);

        Count++;
    }

    public void InsertBack(T item)
    {
        if (Empty)
            Head = Tail = new ListNode(item);
        else
            Tail.Next = Tail = new ListNode(item, null, Tail);

        Count++;
    }

    public void Append(T item, ListNode target)
    {
        if (target == null)
            return;

        if (target.Next == null)
            InsertBack(item);
        else
        {
            target.Next.Previous = target.Next = new ListNode(item, target.Next, target);
            Count++;
        }
    }

    public void Prepend(T item, ListNode target)
    {
        if (target == null)
            return;

        if (target.Previous == null)
            InsertFront(item);
        else
        {
            target.Previous.Next = target.Previous = new ListNode(item, target, target.Previous);
            Count++;
        }
    }

    public T RemoveHead()
    {
        if (Empty)
            return default;

        T selection = Head.Data;
        Head = Head.Next;

        if (Head != null)
            Head.Previous = null;

        Count--;

        return selection;
    }

    public T RemoveTail()
    {
        if (Empty)
            return default;

        T selection = Tail.Data;

        Tail = Tail.Previous;

        if (Tail != null)
            Tail.Next = null;

        Count--;

        return selection;
    }

    public void SendToFront(ListNode node)
    {
        if (Count < 2)
            return;

        if (node == Head)
            return;

        if (node == Tail)
            Tail = node.Previous;
        else
            node.Next.Previous = node.Previous;

        node.Previous.Next = node.Next;
        node.Next = Head;
        Head.Previous = node;
        Head = node;

        node.Previous = null;
    }

    public void SendToBack(ListNode node)
    {
        if (Count < 2)
            return;

        if (node == Tail)
            return;

        if (node == Head)
            Head = node.Next;
        else
            node.Previous.Next = node.Next;

        node.Next.Previous = node.Previous;
        node.Previous = Tail;
        Tail.Next = node;
        Tail = node;

        node.Next = null;
    }

    /// <summary>
    /// Sends head to back of the list.
    /// </summary>
    public void Slap()
    {
        if (Count < 2)
            return;

        Tail.Next = Head;
        Tail.Next.Previous = Tail;
        Tail = Head;
        Head = Tail.Next;
        Head.Previous = null;
        Tail.Next = null;
    }

    /// <summary>
    /// Brings tail to front of the list.
    /// </summary>
    public void Rewind()
    {
        if (Count < 2)
            return;

        Tail.Next = Head;
        Tail.Next.Previous = Tail;
        Head = Tail;
        Tail = Head.Previous;
        Head.Previous = null;
        Tail.Next = null;
    }

    public bool Contains(T item)
    {
        ListNode current = Head;
        while (current != null)
        {
            if (current.Data.Equals(item))
                return true;

            current = current.Next;
        }

        return false;
    }

    public ListNode Find(T item)
    {
        ListNode current = Head;

        while (current != null)
        {
            if (current.Data.Equals(item))
                return current;

            current = current.Next;
        }

        return null;
    }

    public bool DestroyContainingNode(T item)
    {
        ListNode current = Head;

        while (current != null)
        {
            if (current.Data.Equals(item))
            {
                DestroyNode(current);
                return true;
            }

            current = current.Next;
        }

        return false;
    }

    public void DestroyNode(ListNode node)
    {
        if (node == null)
            return;

        if (node == Head)
            DestroyHead();
        else if (node == Tail)
            DestroyTail();
        else
        {
            node.Previous.Next = node.Next;
            node.Next.Previous = node.Previous;

            Count--;
        }
    }

    public void DestroyHead()
    {
        if (Empty)
            return;

        Head = Head.Next;

        if (Head != null)
            Head.Previous = null;

        Count--;
    }

    public void DestroyTail()
    {
        if (Empty)
            return;

        Tail = Tail.Previous;

        if (Tail != null)
            Tail.Next = null;

        Count--;
    }

    public void Perform(System.Action<ListNode> action)
    {
        ListNode current = Head;
        while (current != null)
        {
            action(current);
            current = current.Next;
        }
    }

    public void Clear()
    {
        ListNode current = Head;
        while (current != null)
        {
            ListNode next = current.Next;
            current.Next = null;
            current.Previous = null;
            current = next;
        }

        Count = 0;
        Head = null;
        Tail = null;
    }

    public T[] ToArray()
    {
        T[] array = new T[Count];

        int i = 0;
        ListNode current = Head;

        while (current != null)
        {
            array[i++] = current.Data;
            current = current.Next;
        }

        return array;
    }

    public static DoubleLinkedList<T> FromArray(T[] array)
    {
        DoubleLinkedList<T> list = new DoubleLinkedList<T>();

        for (int i = array.Length - 1; i >= 0; i--)
            list.InsertFront(array[i]);

        return list;
    }

    public override string ToString()
    {
        if (Empty)
            return "List is empty";

        string list = "";

        ListNode current = Head;
        while (current != null)
        {
            list += "[" + current.Data.ToString() + "] ";
            current = current.Next;
        }

        return list;
    }
}
