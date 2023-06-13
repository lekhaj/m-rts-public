using System;
using System.Threading.Tasks;
using UnityEngine;

public class FoodProvider : MonoBehaviour
{
    [SerializeField]
    private Animator _bananFallAnimator;

    public static event Action AddFoodCount;

    private bool calledOnce = false;

    private void Start()
    {
        _bananFallAnimator.enabled = false;
    }

    private void Update()
    {
        if (!calledOnce)
        {
            ActivateAnimator();
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Battle ground"))
        {
            AddFoodCount?.Invoke();
        }
    }

    private void ActivateAnimator()
    {
        calledOnce = true;
        StartAnimation();
    }

    async void StartAnimation()
    {
        await Task.Delay(2000);
        _bananFallAnimator.enabled=true;
        StopAnimation();

    }

    async void StopAnimation()
    {
        await Task.Delay(1000);
        _bananFallAnimator.enabled = false;
        calledOnce = false;
    }

}
