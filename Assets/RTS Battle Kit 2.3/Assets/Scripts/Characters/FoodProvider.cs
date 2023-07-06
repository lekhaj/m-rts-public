using System;
using System.Threading.Tasks;
using UnityEngine;

public class FoodProvider : MonoBehaviour
{
    private Animator _bananFallAnimator;

    public static event Action AddFoodCount;

    private bool calledOnce = false;

    private void Start()
    {
        _bananFallAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!_bananFallAnimator.GetBool("canFall")){
            GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            GetComponent<MeshRenderer>().enabled = true;
        }

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
        if (_bananFallAnimator != null)
        {
            _bananFallAnimator.SetBool("canFall", true);
            StopAnimation();
        }

    }

    async void StopAnimation()
    {
        await Task.Delay(1000);
        if(_bananFallAnimator != null)
        {
            _bananFallAnimator.SetBool("canFall", false);
        }
        calledOnce = false;
    }

}
