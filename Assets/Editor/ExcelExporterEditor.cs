using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MongoDB.Bson;
//using NPOI.SS.UserModel;
//using NPOI.XSSF.UserModel;
using UnityEditor;
using UnityEngine;
using ZMDFQ;
using ExcelDataReader;

public struct CellInfo
{
    public string Type;
    public string Name;
    public string Desc;
}

public class ExcelMD5Info
{
    public Dictionary<string, string> fileMD5 = new Dictionary<string, string>();

    public string Get(string fileName)
    {
        string md5 = "";
        this.fileMD5.TryGetValue(fileName, out md5);
        return md5;
    }

    public void Add(string fileName, string md5)
    {
        this.fileMD5[fileName] = md5;
    }
}

public class ExcelExporterEditor : EditorWindow
{
    [MenuItem("Tools/导出配置")]
    private static void ShowWindow()
    {
        GetWindow(typeof(ExcelExporterEditor));
    }

    private const string ExcelPath = "./Excel";

    private ExcelMD5Info md5Info;

    // Update is called once per frame
    private void OnGUI()
    {
        try
        {
            const string clientPath = "./Assets/Bundle/Data";

            if (GUILayout.Button("导出配置"))
            {
                ExportAll(clientPath);

                Log.Info($"导出配置完成!");
            }
        }
        catch (Exception e)
        {
            Log.Error(e);
        }
    }

    private void ExportAll(string exportDir)
    {
        foreach (string filePath in Directory.GetFiles(ExcelPath))
        {
            if (Path.GetExtension(filePath) != ".xlsx")
            {
                continue;
            }
            if (Path.GetFileName(filePath).StartsWith("~"))
            {
                continue;
            }
            string fileName = Path.GetFileName(filePath);

            Export(filePath, exportDir);
        }

        Log.Info("所有表导表完成");
        AssetDatabase.Refresh();
    }

    private void Export(string fileName, string exportDir)
    {
        using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
            string protoName = Path.GetFileNameWithoutExtension(fileName);

            Log.Info($"{protoName}导表开始");

            var reader = ExcelReaderFactory.CreateReader(file);
            {
                var result = reader.AsDataSet();
                foreach (System.Data.DataTable table in result.Tables)
                {
                    string exportPath = Path.Combine(exportDir, $"{table.TableName}.txt");
                    using (FileStream txt = new FileStream(exportPath, FileMode.Create))
                    {
                        using (StreamWriter sw = new StreamWriter(txt))
                        {
                            ExportSheet(table, sw);
                        };
                    };
                };
            }
            Log.Info($"{protoName}导表完成");
        }
    }

    private void ExportSheet(System.Data.DataTable sheet, StreamWriter sw)
    {
        int cellCount = sheet.Columns.Count;
        
        //sheet.Workbook.GetCreationHelper().

        CellInfo[] cellInfos = new CellInfo[cellCount];

        for (int i = 1; i < cellCount; i++)
        {
            string fieldDesc = GetCellString(sheet, 0, i);
            string fieldName = GetCellString(sheet, 1, i);
            string fieldType = GetCellString(sheet, 2, i);
            cellInfos[i] = new CellInfo() { Name = fieldName, Type = fieldType, Desc = fieldDesc };
        }

        for (int i = 3; i < sheet.Rows.Count; ++i)
        {
            if (GetCellString(sheet, i, 0).StartsWith("#"))
            {
                continue;
            }
            //if (GetCellString(sheet, i, 2) == "")
            //{
            //    continue;
            //}
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            //IRow row = sheet.GetRow(i);
            for (int j = 1; j < cellCount; ++j)
            {
                string desc = cellInfos[j].Desc.ToLower();
                if (desc.StartsWith("#"))
                {
                    continue;
                }

                string fieldValue = GetCellString(sheet, i, j);
                if (fieldValue == "")
                {
                    //Log.Warning($"sheet: {sheet.SheetName} 中有空白字段 {i},{j}");
                    continue;
                }

                if (j > 1)
                {
                    sb.Append(",");
                }

                string fieldName = cellInfos[j].Name;

                if (fieldName == "Id" || fieldName == "_id")
                {
                    fieldName = "_id";
                }

                string fieldType = cellInfos[j].Type;
                sb.Append($"\"{fieldName}\":{Convert(fieldType, fieldValue)}");
            }
            sb.Append("}");
            if (i != sheet.Rows.Count - 1) sb.Append("\n");
            sw.Write(sb.ToString());
        }
    }
    static StringBuilder sbCache = new StringBuilder();
    private static string Convert(string type, string value)
    {
        if (type.EndsWith("Enum[]"))
        {
            Type t = typeof(ZMDFQ.Game).Assembly.GetType(type.Substring(0, type.Length - 2));
            sbCache.Clear();
            string[] sp = value.Split(',');
            foreach (string s in sp)
            {
                sbCache.Append((int)Enum.Parse(t, s));
                sbCache.Append(',');
            }
            sbCache.Remove(sbCache.Length - 1, 1);
            return $"[{sbCache.ToString()}]";
        }
        if (type.EndsWith("Enum"))
        {
            Type t = typeof(ZMDFQ.Game).Assembly.GetType(type);
            return ((int)Enum.Parse(t, value)).ToString();
        }
        switch (type)
        {
            case "int[]":
            case "int32[]":
            case "long[]":
                return $"[{value}]";
            case "string[]":
                sbCache.Clear();
                string[] sp = value.Split(',');
                foreach (string s in sp) sbCache.Append($"\"{s}\",");
                sbCache.Remove(sbCache.Length - 1, 1);
                return $"[{sbCache.ToString()}]";
            case "int":
            case "int32":
            case "int64":
            case "long":
            case "float":
            case "double":
                return value;
            case "string":
                return $"\"{value}\"";
            default:
                if (type.EndsWith("[]"))
                {
                    return $"[{value}]";
                }
                return value;
                //throw new Exception($"不支持此类型: {type}");
        }
    }

    private static string GetCellString(System.Data.DataTable sheet, int i, int j)
    {
        return sheet.Rows[i][j].ToString();
    }
}
