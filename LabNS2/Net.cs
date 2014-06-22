using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LabNS2
{
    class Net
    {
        //создание сети с заданным разрешением изображения (количество точек)
        public Net(int pointCount)
        {
            resolution = pointCount;
        }

        //сохранение всех нейронов в файл
        public void Save()
        {
            foreach (Neuron n in net)
                n.Save();
        }

        int resolution;
        List<Neuron> net = new List<Neuron>();

        //добавление нейрона - create
        public void AddSomeClass(char symbol)
        {
            net.Add(new Neuron(symbol, resolution));
        }
        int lastSomeClass;

        //распознавание  - вызов сравнения и поиск
        public char Recognize(List<byte> x)
        {
            char result = '\n';
            lastSomeClass = -1;
            double max = 0;
            foreach (Neuron n in net)
            {
                double Candidate = n.Y(x);
                if (Candidate > max)
                {
                    max = Candidate;
                    result = n.Symbol;
                    lastSomeClass = net.IndexOf(n);
                }
            }
            return result;
        }


        //обучение
        public char Correct(char symbol, List<byte> x, double speed)
        {
            bool check = false;
            for (int i = 0; i < net.Count; i++)
            {
                if (symbol == '\n')
                {
                    if (net[i].LastY > 0)
                        net[i].Correct(x, -1, speed);
                }
                else
                {
                    if (net[i].Symbol == symbol)
                    {
                        net[i].Correct(x, 1, speed);
                        check = true;
                    }
                    else if (net[i].LastY > 0)
                        net[i].Correct(x, -1, speed);
                }
            }
            if (!check)
            {
                net.Add(new Neuron(symbol, x.Capacity - 1));
                net[net.Count - 1].Correct(x, 1, speed);
                StreamWriter write = new StreamWriter("chars.txt", true);
                write.WriteLine(symbol);
                write.Close();
            }
            return Recognize(x);
        }

        //данные о нейроне
        public List<double> GetSomeClass(char s)
        {
            foreach (Neuron n in net)
                if (n.Symbol == s)
                    return n.w;
            return null;
        }

       
    }
}
