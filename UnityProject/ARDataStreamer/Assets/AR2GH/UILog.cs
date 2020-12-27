using TMPro;
using UnityEngine;

public class UILog : MonoBehaviour
{
    [SerializeField] private TMP_Text _tmp = null;
    
    private void Start()
    {
        Application.logMessageReceivedThreaded += HandleLog;
    }

    private void HandleLog(string logString, string stacktrace, LogType type)
    {
        _tmp.text = $"{logString}\n{_tmp.text}";
    }
}
