using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class DataPlotter : MonoBehaviour {

    // Name of the input file, no extension
    public string inputfile;
    private int fileIndex = 0;

    //System.Timers.Timer timer = new System.Timers.Timer();

    // List for holding data from CSV reader
    private List<Dictionary<string, object>> pointList;

    // Indices for columns to be assigned
    public int columnX = 2; // 0;
    public int columnY = 1; // 1;
    public int columnZ = 3;

    public int informationIndex_1 = 3;
    public int informationIndex_2 = 4;

    // Full column names
    public string xName;
    public string yName;
    public string zName;

    public string informationLabel_1;
    public string informationLabel_2;

    public float plotScale = 10;

    // The prefab for the data points that will be instantiated
    public GameObject PointPrefab;
    public GameObject LabelPrefab;

    // Object which will contain instantiated prefabs in hiearchy
    public GameObject PointHolder;

    float xMax;
    float yMax;
    float zMax;

    // Get minimums of each axis
    float xMin;
    float yMin;
    float zMin;


    // Use this for initialization
    void Start () {

        createAxis(1, 0, 0);
        createAxis(0, 1, 0);
        createAxis(0, 0, 1);

        createAxisLabel("x", 10);
        createAxisLabel("y", 10);
        createAxisLabel("z", 10);

        pointList = CSVReader.Read(inputfile);

        // Declare list of strings, fill with keys (column names)
        List<string> columnList = new List<string>(pointList[1].Keys);

        // Print number of keys (using .count)
        Debug.Log("There are " + columnList.Count + " columns in the CSV");

        foreach (string key in columnList)
            // Debug.Log("Column name is " + key);

            // Assign column name from columnList to Name variables
            xName = columnList[columnX];
            yName = columnList[columnY];
            zName = columnList[columnZ];

            informationLabel_1 = columnList[informationIndex_1];
            informationLabel_2 = columnList[informationIndex_2];

        // Get maxes of each axis
        xMax = FindMaxValue(xName);
        yMax = FindMaxValue(yName);
        zMax = FindMaxValue(zName);

        // Get minimums of each axis
        xMin = FindMinValue(xName);
        yMin = FindMinValue(yName);
        zMin = FindMinValue(zName);


        StartCoroutine("createGraph");
    }

    void Update()
    {
        if (fileIndex >= 0)
        {
            // createGraph(fileIndex);
            //StartCoroutine("createGraph");
        }
    }

    IEnumerator createGraph()
    {

        //Loop through Pointlist
        for (var i = 0; i < pointList.Count; i++)
        {
            // Get value in poinList at ith "row", in "column" Name, normalize
            float x =
                (System.Convert.ToSingle(pointList[i][xName]) - xMin)
                / (xMax - xMin);

            float y =
                (System.Convert.ToSingle(pointList[i][yName]) - yMin)
                / (yMax - yMin);

            float z =
                (System.Convert.ToSingle(pointList[i][zName]) - zMin)
                / (zMax - zMin);


            // Instantiate as gameobject variable so that it can be manipulated within loop
            GameObject dataPoint = Instantiate(
                    PointPrefab,
                    new Vector3(x, y, z) * plotScale,
                    Quaternion.identity);

            // Make child of PointHolder object, to keep points within container in hiearchy
            dataPoint.transform.parent = PointHolder.transform;

            // Assigns original values to dataPointName
            string dataPointName =
                pointList[i][xName] + " "
                + pointList[i][yName] + " "
                + pointList[i][zName];

            // Assigns name to the prefab
            dataPoint.transform.name = dataPointName;

            // Gets material color and sets it to a new RGB color we define
            dataPoint.GetComponent<Renderer>().material.color =
                new Color(x, y, z, 1.0f);


            GameObject dataLabel = Instantiate(LabelPrefab, new Vector3(x, y, z) * plotScale, Quaternion.identity);
            dataLabel.transform.parent = PointHolder.transform;
            string MSTeamsUsageKeyText = pointList[i][informationLabel_1] + "/" + pointList[i][informationLabel_2];
            dataLabel.GetComponentInChildren<TextMesh>().text = MSTeamsUsageKeyText;

            createLine(x, y, z);

            Debug.Log("Debug Count " + i + "/" + pointList.Count + " Rows printed.");

            if (i < 90000)
			{
				if (i % 10000 == 0)
				{
					Debug.Log(i + "/" + pointList.Count + "========================================================================");
					yield return new WaitForSecondsRealtime(0.01f);
				}

			}
			else
			{
				if (i % 100 == 0)
				{
					Debug.Log(i + "/" + pointList.Count + "========================================================================");
					yield return new WaitForSecondsRealtime(0.01f);
				}

			}
		}   
    }

    private void createLine(float x, float y, float z)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = new Vector3(x, y, z) * plotScale;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        // lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        // lr.SetColors(Color, '#0F0'), Color, 'red);
        lr.GetComponent<Renderer>().material.color = new Color(x, y, z, 1.0f);

        //lr.SetWidth(0.1f, 0.1f);
        lr.startWidth = 0.01f;
        lr.endWidth = 0.01f;

        lr.SetPosition(0, new Vector3(x, 0, z) * plotScale);
        lr.SetPosition(1, new Vector3(x, y, z) * plotScale);
        
        Vector3 lineR = lr.GetPosition(0);
        
        // Debug.Log("lineR value of 0 is " + lineR.x + "/" + lineR.y + "/" + lineR.z + " ::: " + "User Name " + NodeName);

        Vector3 lineL = lr.GetPosition(1);
    }

    private void createAxisLabel(string axis, float maxValue)
    {
        
        switch (axis)
        {
            case "x":
                //float xMax = maxValue;
                for (int i = 0; i < maxValue; i++)
                {
                    GameObject dataLabel = Instantiate(LabelPrefab, new Vector3(i, -1, 0), Quaternion.identity);
                    dataLabel.transform.parent = PointHolder.transform;
                    string MSTeamsUsageKeyText = "X-" + i; //pointList[i][informationLabel_1] + "/" + pointList[i][informationLabel_2];
                    dataLabel.GetComponentInChildren<TextMesh>().text = MSTeamsUsageKeyText;

                }
                break;
            case "y":
                for (int i = 0; i < maxValue; i++)
                {
                    GameObject dataLabel = Instantiate(LabelPrefab, new Vector3(-1, i, 0), Quaternion.identity);
                    dataLabel.transform.parent = PointHolder.transform;
                    string MSTeamsUsageKeyText = "Y-" + i; //pointList[i][informationLabel_1] + "/" + pointList[i][informationLabel_2];
                    dataLabel.GetComponentInChildren<TextMesh>().text = MSTeamsUsageKeyText;

                }
                break;
            case "z":
                for (int i = 0; i < maxValue; i++)
                {
                    GameObject dataLabel = Instantiate(LabelPrefab, new Vector3(-1, 0, i), Quaternion.identity);
                    dataLabel.transform.parent = PointHolder.transform;
                    string MSTeamsUsageKeyText = "Z-" + i; //pointList[i][informationLabel_1] + "/" + pointList[i][informationLabel_2];
                    dataLabel.GetComponentInChildren<TextMesh>().text = MSTeamsUsageKeyText;

                }

                break;
        }
    }

    private void createAxis(float x, float y, float z)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = new Vector3(x, y, z) * plotScale;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        // lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        // lr.SetColors(Color, '#0F0'), Color, 'red);
        lr.GetComponent<Renderer>().material.color = new Color(255, 255, 255, 1.0f);

        //lr.SetWidth(0.1f, 0.1f);
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;

        lr.SetPosition(0, new Vector3(0, 0, 0) * plotScale);
        lr.SetPosition(1, new Vector3(x, y, z) * plotScale);

        Vector3 lineR = lr.GetPosition(0);
        // Debug.Log("lineR value of 0 is " + lineR.x + "/" + lineR.y + "/" + lineR.z + " ::: " + "User Name " + NodeName);

        Vector3 lineL = lr.GetPosition(1);
    }

    private float FindMaxValue(string columnName)
    {
        //set initial value to first value
        float maxValue = Convert.ToSingle(pointList[0][columnName]);

        //Loop through Dictionary, overwrite existing maxValue if new value is larger
        for (var i = 0; i < pointList.Count; i++)
        {
            if (maxValue < Convert.ToSingle(pointList[i][columnName]))
                maxValue = Convert.ToSingle(pointList[i][columnName]);
        }

        //Spit out the max value
        return maxValue;
    }

    private float FindMinValue(string columnName)
    {

        float minValue = Convert.ToSingle(pointList[0][columnName]);

        //Loop through Dictionary, overwrite existing minValue if new value is smaller
        for (var i = 0; i < pointList.Count; i++)
        {
            if (Convert.ToSingle(pointList[i][columnName]) < minValue)
                minValue = Convert.ToSingle(pointList[i][columnName]);
        }

        return minValue;
    }

}
