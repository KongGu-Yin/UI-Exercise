using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

public class TableTools
{
    private const string xmlRoot    = "/StreamingAssets/ConfigXML/";

    private const string scriptRoot = "/Scripts/ConfigTable/";

    [MenuItem("Config/GenerateConfig[生成表模板]")]
    public static void GenerateTableEntity()
    {
        DirectoryInfo xmlFolder = new DirectoryInfo(Application.dataPath + xmlRoot);
        List<string> tableClassNames = new List<string>();
        ClearTableClass();
        ClearTableTypes();
        foreach (FileInfo curFile in xmlFolder.GetFiles())
        {
            if (curFile.Name.Substring(curFile.Name.Length - 4, 4) == ".xml")
            {

                Dictionary<string, string> values = LoadXmlTypes(curFile.Name, "fieldType");
                if (values == null)
                    return;

                bool hasAllType = CheckTypes(curFile.Name, values);

                if (hasAllType == false)
                    return;

                string className = curFile.Name.Substring(0, curFile.Name.Length - 4);


                //将类名 . 替换成_
                className = className.Replace('.', '_');
                tableClassNames.Add(className);
                Dbg.INFO_MSG("Create TableClass :" + className + ".cs");
                CreateTableClass(className, values);
            }
        }
        CreateTableTypes(tableClassNames.ToArray());
        AssetDatabase.Refresh();
        Dbg.INFO_MSG("GenerateTableEntity Successfull !!!");
    }

    private static void ClearTableTypes()
    {
        CreateTableTypes(null);
    }

    private static void ClearTableClass()
    {
        if (Directory.Exists(Application.dataPath + scriptRoot + "datas/"))
        {
            DelectDir(Application.dataPath + scriptRoot + "datas/");
        }
    }

    public static void DelectDir(string srcPath)
    {
        try
        {
            DirectoryInfo dir = new DirectoryInfo(srcPath);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
            foreach (FileSystemInfo i in fileinfo)
            {
                if (i is DirectoryInfo)            //判断是否文件夹
                {
                    DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                    subdir.Delete(true);          //删除子目录和文件
                }
                else
                {
                    File.Delete(i.FullName);      //删除指定文件
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }


    public static Dictionary<string, string> LoadXmlTypes(string xmlname, string itemName = null)
    {
        XElement xml = XElement.Load((Application.dataPath + xmlRoot + xmlname));
        if (! xml.HasElements)
        {
            Dbg.ERROR_MSG("xml 丢失元素");
            return null;
        }
        Dictionary<string, string> xmltypes = new Dictionary<string, string>();
        var descendants =  xml.Descendants();
        foreach (XElement item in descendants)
        {
            if (itemName == null || item.Name == itemName)
            {
                var attrs = item.Attributes();
                foreach (XAttribute attr in attrs)
                {
                    xmltypes.Add(attr.Name.ToString(), attr.Value);
                }
            }
        }
        return xmltypes;
    }

    public static void CreateTableClass(string className, Dictionary<string, string> values)
    {
        FileStream fs = new FileStream(Application.dataPath + scriptRoot + "datas/" + className + ".cs", FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);
        //开始写入
        sw.WriteLine("//The Data is Automatic generation, Don't Modify It !!!");
        sw.WriteLine("public struct " + className);
        sw.WriteLine("{");
        if (values != null)
        {
            foreach (string fileName in values.Keys)
            {
                sw.WriteLine("    public " + values[fileName] + " " + fileName + ";");
                sw.WriteLine("");
            }
        }
        sw.WriteLine("}");

        //清空缓冲区
        sw.Flush();
        //关闭流
        sw.Close();
        fs.Close();
    }

    public static void CreateTableTypes(string[] tableTypes)
    {
        FileStream fs = new FileStream(Application.dataPath + scriptRoot  + "TableTypes.cs", FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);
        //开始写入  CS MOD
        sw.WriteLine("//The Class is Automatic generation, Don't Modify It !!!");
        sw.WriteLine("using System;");
        sw.WriteLine("using System.Collections.Generic;");
        sw.WriteLine("");
        sw.WriteLine("");
        sw.WriteLine("public static class TableTypes");
        sw.WriteLine("{");
        sw.WriteLine("      public static List<Type> GetTableTypes()");
        sw.WriteLine("      {");
        sw.WriteLine("          List<Type> tabletypes = new List<Type>();");
        if (tableTypes != null)
        {
            foreach (string curTableType in tableTypes)
            {
                sw.WriteLine("          tabletypes.Add(typeof(" + curTableType + "));");
            }
        }

        sw.WriteLine("          return tabletypes;");
        sw.WriteLine("      }");
        sw.WriteLine("}");

        //清空缓冲区
        sw.Flush();
        //关闭流
        sw.Close();
        fs.Close();
    }

    public static bool CheckTypes(string xmlName, Dictionary<string, string> values)
    {
        foreach (string type in values.Values)
        {
            if (Type.GetType(type) == null)
            {
                Dbg.ERROR_MSG("类型检测错误！  未能识别此类型:" + type + "     >>" + xmlName);
                return false;
            }
        }
        return true;
    }

}
