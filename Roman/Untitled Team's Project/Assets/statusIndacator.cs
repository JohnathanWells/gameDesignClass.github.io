using UnityEngine;
using UnityEngine.UI;

public class statusIndacator : MonoBehaviour {

    public RectTransform healthBarRect;
    public Text healthText;



	// Use this for initialization
	void Start () {

        if (healthBarRect == null)
        {
            Debug.LogError("STAUTS INDAICATOR: No health bar object referenced!");
        }
	
	}

    public void SetHealth(int _cur, int _max)
    {
        float _value = (float)_cur / _max;

        healthBarRect.localScale = new Vector3(_value, healthBarRect.localScale.y, healthBarRect.localScale.z);
        healthText.text = _cur + "/" + _max + "HP";
    }
	
}
