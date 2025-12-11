using UnityEngine;
using System.IO.Ports;

public class ArduinoSerial : MonoBehaviour
{
    public string portName = "COM7";
    public int baudRate = 115200;

    SerialPort port;

    public int distancia;
    public float powerArduino; // de 0 a 10

    void Start()
    {
        port = new SerialPort(portName, baudRate);
        port.ReadTimeout = 50;

        try { port.Open(); }
        catch { Debug.LogError("No se pudo abrir el puerto " + portName); }
    }

    void Update()
    {
        if (port == null || !port.IsOpen) return;

        try
        {
            string line = port.ReadLine();
            ProcesarJSON(line);
        }
        catch { }
    }

    void ProcesarJSON(string json)
    {
        // Espera formato {"dist":45,"power":4}
        json = json.Replace("{", "").Replace("}", "");

        string[] pares = json.Split(',');

        foreach (string p in pares)
        {
            string[] kv = p.Split(':');
            if (kv[0].Contains("dist"))
                distancia = int.Parse(kv[1]);
            else if (kv[0].Contains("power"))
                powerArduino = float.Parse(kv[1]);
        }
    }

    public void EnviarEstado(int vidas)
{
    if (port == null || !port.IsOpen) return;

    int tiempo = Timer.Instance.GetCurrentSeconds();

    string msg = "L:" + vidas + ";T:" + tiempo + "\n";
    port.Write(msg);
}

    private void OnApplicationQuit()
    {
        if (port != null && port.IsOpen)
            port.Close();
    }
}
