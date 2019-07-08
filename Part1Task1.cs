using System;
using System.IO;//библиотека для работы с файлами

namespace ConsoleApp10
{
    class Part1Task1
    {
        static string engAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";//из этой строки будем генерировать 10 букв для каждой строки
        static string rusAlphabet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя";//аналгично предыдущей строке
        static Random rnd = new Random();//эта переменная служит для генерации

        public static string stringGeneration()//функция, генерирующая одну строку
        {
            string s="";
            DateTime start = DateTime.Today.AddYears(-5);//нижняя граница для генерируемой даты - ровно за 5 лет до начала генерации
            int range = (DateTime.Today - start).Days;//количество дней, которые мы можем получить в ходе генерации
            s += start.AddDays(rnd.Next(range)).ToShortDateString() + "||";//генерируем непосредственно дату, прибавляя к нижней границе случайное число от 0 до максимального количества дней(см. предыдущую строку
            for (int i = 0; i < 10; i++)//в данном цикле генерируем 10 латинских букв
            {               
                int symb = rnd.Next(0, engAlphabet.Length - 1);
                s += engAlphabet[symb];
            }
            s += "||";
            for (int i = 0; i < 10; i++)//в этом цикле генерируем кириллические буквы
            {
                int symb = rnd.Next(0, rusAlphabet.Length - 1);
                s += rusAlphabet[symb];
            }
            s += "||";
            s += (rnd.Next(1, 50000000) * 2).ToString() + "||";//генерация чётного числа 
            double dn = rnd.NextDouble()*19+1;//генерация дробного числа, строим биекцию между множествами [0;1] и [1;20]
            s +=(Math.Round(dn,8)).ToString() + "||";//округляем до 8 знаков после запятой
            return s;
        }
        static void Main(string[] args)
        {
            for (int i = 0; i < 100; i++)
            {
                string path = "File" + (i+1).ToString() + ".txt";//создаём множество имён файлов
                using (StreamWriter sw = new StreamWriter(path))
                {
                    for (int j = 0; j < 100000; j++)
                    {
                        sw.WriteLine(stringGeneration());//добавляем по 100000 строк в каждый файл
                    }
                }
            }
        }
    }
}
