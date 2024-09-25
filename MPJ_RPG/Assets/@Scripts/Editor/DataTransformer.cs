using Data;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class DataTransformer : EditorWindow
{
#if UNITY_EDITOR
	//메뉴버튼 생성+단축키(Ctrl+Shift+K)
	[MenuItem("Tools/ParseExcel %#K")]
	public static void ParseExcelDataToJson()
	{
		ParseExcelDataToJson<HeroDataLoader, HeroData>("Hero");
		ParseExcelDataToJson<MonsterDataLoader, MonsterData>("Monster");
		ParseExcelDataToJson<EnvDataLoader, EnvData>("Env");
		ParseExcelDataToJson<SkillDataLoader, SkillData>("Skill");
		ParseExcelDataToJson<ProjectileDataLoader, ProjectileData>("Projectile");

		Debug.Log("DataTransformer Completed");
	}

	#region Helpers
	//where Loader : new()와 where LoaderData : new()는 Loader와 LoaderData 형식이 기본 생성자를 가져야 한다는 제약 조건을 나타냅니다. 즉, 이 함수 내에서 Loader와 LoaderData는 반드시 인스턴스화할 수 있어야 합니다.
	private static void ParseExcelDataToJson<Loader, LoaderData>(string filename) where Loader : new() where LoaderData : new()
	{
		Loader loader = new Loader();
		//용례 : TestDataLoader에서 List<TestData> tests
		FieldInfo field = loader.GetType().GetFields()[0];
		field.SetValue(loader, ParseExcelDataToList<LoaderData>(filename));

		//loader 객체를 json 파일로 직렬화
		string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);

		//{Application.dataPath}는 Unity 프로젝트의 데이터 폴더 경로를 나타냅니다.
		File.WriteAllText($"{Application.dataPath}/@Resources/Data/JsonData/{filename}Data.json", jsonStr);
		AssetDatabase.Refresh();
	}

	// 파싱 : 파일 내용을 읽어와서 데이터 구조로 변환하는 과정
	private static List<LoaderData> ParseExcelDataToList<LoaderData>(string filename) where LoaderData : new()
	{
		List<LoaderData> loaderDatas = new List<LoaderData>();

		//경로에 있는 파일을 읽어서 string 배열에 저장
		string[] lines = File.ReadAllText($"{Application.dataPath}/@Resources/Data/ExcelData/{filename}Data.csv").Split("\n");

		//string 배열을 순회하며 행별로 파싱해 필드 값에 저장
		for (int l = 1; l < lines.Length; l++)
		{
			string[] row = lines[l].Replace("\r", "").Split(',');
			if (row.Length == 0)
				continue;
			if (string.IsNullOrEmpty(row[0]))
				continue;

			LoaderData loaderData = new LoaderData();

			FieldInfo[] fields = loaderData.GetType().GetFields();
			for (int f = 0; f < fields.Length; f++)
			{
				FieldInfo field = loaderData.GetType().GetField(fields[f].Name);
				Type type = field.FieldType;

				//참고) 컬렉션(데이터 그룹) : 배열, 리스트, set, 딕셔너리
				//List<>, Dictionary<> ... 모두 제네릭 타입
				if (type.IsGenericType)
				{
					object value = ConvertList(row[f], type);
					field.SetValue(loaderData, value);
				}
				else
				{
					object value = ConvertValue(row[f], type);
					field.SetValue(loaderData, value);
				}
			}

			loaderDatas.Add(loaderData);
		}

		return loaderDatas;
	}

	//value(문자열)를 type 형식으로 변환
	private static object ConvertValue(string value, Type type)
	{
		if (string.IsNullOrEmpty(value))
			return null;

		TypeConverter converter = TypeDescriptor.GetConverter(type);
		return converter.ConvertFromString(value);
	}

	private static object ConvertList(string value, Type type)
	{
		if (string.IsNullOrEmpty(value))
			return null;

		// Reflection
		Type valueType = type.GetGenericArguments()[0]; //제네릭 변수의 첫번째 인자 타입
		Type genericListType = typeof(List<>).MakeGenericType(valueType); //List<valueType> 형식의 타입 생성
		var genericList = Activator.CreateInstance(genericListType) as IList; //리스트 인스턴스 생성

		// Parse Excel
		var list = value.Split('&').Select(x => ConvertValue(x, valueType)).ToList();

		foreach (var item in list)
			genericList.Add(item);

		return genericList;
	}
	#endregion
#endif
}
