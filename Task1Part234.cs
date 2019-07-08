//Важной частью данного кода является его часть с использованием SqlBulkCopy. Прблема в том, что для добавления 100000 записей в 
//БД SQL напрямую(то есть,добавляя непосредственно в БД по одной записи), то на это уйдёт довольно большое время, для добавления же всех
//возможных записей, коих будет 10000000, уйдёт явно более суток, отчего придётся добавлять сначла по большому количеству записей в
//DataTable, а оттуда с использованием вышеупомянутого SqlBulkCopy уже в БД
using System;
using System.IO;//библиотека для работы с файлами
using System.Data;
using System.Data.SqlClient;//библиотека для работы с SQL

namespace ConsoleApp11
{
    class Task1Part234
    {        
        public static void unionFiles(string[] paths, string symbols, string path)//объединение нескольких файлов в один с удалением строк
        {
            int delStrings = 0;//счётчикудалённых строк
            using (StreamWriter sw = new StreamWriter(path))
            {
                for (int i = 0; i < paths.Length; i++)
                {
                    using (StreamReader sr = new StreamReader(paths[i]))
                    {
                        for (int j = 0; j < 100000; j++)//знаем, что строк в файлах ровно 100000
                        {
                            string s = sr.ReadLine();
                            if (s.Contains(symbols))//если в строке содержится искомая последовательность символов, то не добавляем строку...
                            {
                                delStrings++;//...и увеличиваем счётчик удалённых строк
                            }
                            else//если же такой последовательности символов нет...
                            {
                                sw.WriteLine(s);//... то вносим в новый файл строку
                            }
                        }
                    }
                    sw.WriteLine("Объединение файлов произведено. В процессе было удалено " + delStrings.ToString() + " строк");//в конце вносим информация об удалённых строках
                }
            }
        }

        public static void AddToDatabase(string path)//добавление в базу данных
        {
            int maxSize = 10000;//не будем добавлять в БД за раз более 10000 раз
            DataTable timeData = new DataTable();//данный объект будет временно хранить наши строки
            using (StreamReader sr = new StreamReader(path))
            {
                //до следующего комментария - создание колонок для DataTable
                DataColumn datecolumn1 = new DataColumn("date", Type.GetType("System.DateTime"));
                datecolumn1.AllowDBNull = true;
                timeData.Columns.Add(datecolumn1);
                DataColumn datecolumn2 = new DataColumn("engSymbols", Type.GetType("System.String"));
                datecolumn2.AllowDBNull = true;
                timeData.Columns.Add(datecolumn2);
                DataColumn datecolumn3 = new DataColumn("rusSymbols", Type.GetType("System.String"));
                datecolumn3.AllowDBNull = true;
                timeData.Columns.Add(datecolumn3);
                DataColumn datecolumn4 = new DataColumn("integerNumber", Type.GetType("System.Int64"));
                datecolumn4.AllowDBNull = true;
                timeData.Columns.Add(datecolumn4);
                DataColumn datecolumn5 = new DataColumn("doubleNumber", Type.GetType("System.Double"));
                datecolumn5.AllowDBNull = true;
                timeData.Columns.Add(datecolumn5);
                int k = 0;//счётчик для числа добавленных строк
                string line;
                while ((line = sr.ReadLine()) != null)//считываем строки, пока файл не закончится
                {
                    string[] parts = line.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);//разбиваем строку на подстроки, разделителем будет символ '|'
                    string[] dateStrings = parts[0].Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);//разбиваем дату на части
                    timeData.Rows.Add(new object[] { Convert.ToDateTime(dateStrings[0]+"/"+dateStrings[1]+"/"+dateStrings[2]),
                        parts[1], parts[2], Convert.ToInt64(parts[3]), Convert.ToDouble(parts[4]) });//добавление строки в DataTable
                    k++;//увеличиваем счётчик добавленных строк
                    if (k == maxSize)//если уже добавили 10000 строк...
                    {
                        k = 0;//...обнуляем счётчик...
                        AddFromDataTable(timeData);//...см. функцию ниже...
                        timeData.Rows.Clear();//...и очищаем DataTable...
                    }
                }
                if (k != 0)//если остались строки, то добавляем и их
                {
                    AddFromDataTable(timeData);
                    timeData.Rows.Clear();
                }
            }
        }

        static private void AddFromDataTable(DataTable timeData)//функция добавления из DataTable в БД
        {
            using (SqlConnection dbConnection = new SqlConnection("Data Source = (LocalDB)\\MSSQLLocalDB; Initial Catalog = DatabasesMain; Integrated Security = True; "))//создаём подключение
            {
                dbConnection.Open();//открываем подключение
                using (SqlBulkCopy s = new SqlBulkCopy(dbConnection))//создаём SqlBulkCopy, который сильно ускорит добавление строк в БД
                {
                    s.DestinationTableName = "dbo.Strings";//таблица из БД, в которой мы будем хранить строки
                    foreach (var column in timeData.Columns)
                    {
                        s.ColumnMappings.Add(column.ToString(), column.ToString());//создаём колонки
                    }
                    s.WriteToServer(timeData);//записываем в БД
                }
            }
        }

        public static void AddToDatabase(string[] paths)//добавление для массивафайлов, работает аналогично
        {
            for (int i = 0; i < paths.Length; i++)
            {
                AddToDatabase(paths[i]);
            }
        }

        public static void IntegerSum()//сумма всех целых чисел в БД, вместится в Int64
        {
            //в БД для начала нужно добавить хранимую процедуру со следующим кодом:
            //CREATE PROCEDURE [dbo].[IntegerSum]
            //@sum bigint out
            //AS
            //SELECT @sum = SUM(integerNumber) FROM Strings
            string sqlProc = "IntegerSum";//название процедуры
            using (SqlConnection dbConnection = new SqlConnection("Data Source = (LocalDB)\\MSSQLLocalDB; Initial Catalog = DatabasesMain; Integrated Security = True; "))
            {
                dbConnection.Open();//открываем подключение к БД
                SqlCommand command = new SqlCommand(sqlProc, dbConnection);//переменная с процедурой
                command.CommandType = CommandType.StoredProcedure;//тип процедуры - хранимая процедура
                SqlParameter sum = new SqlParameter
                {
                    ParameterName = "@sum",//ссылаемся на выходной параметр..
                    SqlDbType = SqlDbType.BigInt//...типа BigInt, в который точно вместится сумма
                };
                sum.Direction = ParameterDirection.Output;//тип парметра - выходной
                command.Parameters.Add(sum);//добавляем сумму
                command.ExecuteNonQuery();
                Console.WriteLine("Сумма всех целых чисел в таблице "+command.Parameters["@sum"].Value);//выводим результат на экран
            }
        }

        public static void DoubleMed()//функция поиска медианы,комментарии к строкам аналогично функции суммв
        {
            //CREATE PROCEDURE [dbo].[DoubleMed]
            //@med float out
            //AS SELECT @med = PERCENTILE_CONT(0.5) // процентили, разбивает массив в некотором отношении, в данном случае - на две равных
            //WITHIN GROUP(ORDER BY doubleNumber)
            //OVER()
            //FROM Strings
            string sqlProc = "DoubleMed";
            using (SqlConnection dbConnection = new SqlConnection("Data Source = (LocalDB)\\MSSQLLocalDB; Initial Catalog = DatabasesMain; Integrated Security = True; "))
            {
                dbConnection.Open();
                SqlCommand command = new SqlCommand(sqlProc, dbConnection);
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter med = new SqlParameter
                {
                    ParameterName = "@med",
                    SqlDbType = SqlDbType.Float
                };
                med.Direction = ParameterDirection.Output;
                command.Parameters.Add(med);
                command.ExecuteNonQuery();
                Console.WriteLine("Медиана всех дробных чисел в таблице " + command.Parameters["@med"].Value);
            }
        }

        static void Main(string[] args)
        {            
            string[] paths = new string[2];
            paths[0] = "File1.txt";
            paths[1] = "File2.txt";
            unionFiles(paths, "IR", "Res.txt");
            AddToDatabase("File1.txt");
            IntegerSum();
            DoubleMed();
        }
    }
}
