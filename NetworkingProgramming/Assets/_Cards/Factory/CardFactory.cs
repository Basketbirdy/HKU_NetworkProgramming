using UnityEngine;

public class CardFactory : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    public GameObject CreateCard(CardSO data)
    {
        if(prefab == null) { Debug.LogWarning("No prefab set! can not create card"); }

        GameObject obj = Object.Instantiate(prefab);
        IInitializable<CardSO> initializable = obj.GetComponent<IInitializable<CardSO>>();
        initializable.Init(data);
        
        return obj;
    }
}
