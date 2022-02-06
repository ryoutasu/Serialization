using System;
using System.IO;
using System.Text;

class ListNode
{
    public ListNode Prev;
    public ListNode Next;
    public ListNode Rand;
    public string Data;
}

class ListRand
{
    public ListNode Head;
    public ListNode Tail;
    public int Count;

    public void Fill(int count)
    {
        Random rnd = new Random();
        List<ListNode> nodes = new List<ListNode>();

        Head = new ListNode();
        ListNode node = Head;
        for (int i = 0; i < count; i++)
        {
            node.Data = string.Format("Data for node {0}", i);
            nodes.Add(node);
            if (i < count-1)
            {
                node.Next = new ListNode();
                node.Next.Prev = node;
                node = node.Next;
            }
        }
        Tail = node;
        Count = count;

        foreach (var _node in nodes)
        {
            int r = rnd.Next(0, count-1);
            _node.Rand = nodes[r];
        }
    }
    public void Serialize(FileStream s)
    {
        Dictionary<ListNode, int> dictionary = new Dictionary<ListNode, int>();
        int id = 0;
        for (ListNode currentNode = Head; id < Count; currentNode = currentNode.Next)
        {
            dictionary.Add(currentNode, id);
            id++;
        }

        using (BinaryWriter writer = new BinaryWriter(s))
        {
            writer.Write(Count);
            foreach (var node in dictionary)
            {
                writer.Write(dictionary[node.Key.Rand]);
                writer.Write(node.Key.Data);
            }
        }
        Console.WriteLine("Serialized!");
    }

    public void Deserialize(FileStream s)
    {
        Dictionary<int, Tuple<ListNode,int>> dictionary = new Dictionary<int, Tuple<ListNode, int>>();
        
        int count;
        using (BinaryReader reader = new BinaryReader(s))
        {
            count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                ListNode node = new ListNode();
                int rand = reader.ReadInt32();
                node.Data = reader.ReadString();
                dictionary.Add(i, new Tuple<ListNode, int>(node, rand));
            }
        }
        Count = count;
        Head = dictionary[0].Item1;

        ListNode current = Head;        
        for (int i = 0; i < Count; i++)
        {
            int rand = dictionary[i].Item2;
            current.Rand = dictionary[rand].Item1;
            if (i < Count-1)
            {
                current.Next = dictionary[i+1].Item1;
                current.Next.Prev = current;
                current = current.Next;
            }
        }
        Tail = current;
        Console.WriteLine("Deserialized!");
    }

    public void Print()
    {
        Dictionary<ListNode, int> dictionary = new Dictionary<ListNode, int>();
        int id = 0;
        for (ListNode currentNode = Head; id < Count; currentNode = currentNode.Next)
        {
            dictionary.Add(currentNode, id);
            id++;
        }
        foreach (var node in dictionary)
        {
            Console.WriteLine(node.Key.Data);
            Console.WriteLine("Node.Rand = {0}", dictionary[node.Key.Rand]);
        }
        Console.WriteLine("Count = {0}", Count);
    }
}

class Test
{
    static string path = @"c:\temp\MyTest.txt";
    public static void Main()
    {
        ListRand list = new ListRand();

        Serialize(list);
        //Deserialize(list);

        list.Print();
        Console.ReadLine();
    }

    public static void Serialize(ListRand list)
    {
        list.Fill(30);
        // Delete the file if it exists.
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        // // Create the file.
        using (FileStream fs = File.Create(path))
        {
            list.Serialize(fs);
        }
    }

    public static void Deserialize(ListRand list)
    {
        using (FileStream fs = File.OpenRead(path))
        {
            list.Deserialize(fs);
        }
    }
}