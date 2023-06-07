using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

//GOOGLE MOBILE ADS PACKAGE: https://github.com/googleads/googleads-mobile-unity/releases

//Remove /* and */ after importing the package and enable line 11 & 12

//using GoogleMobileAds;
//using GoogleMobileAds.Api;

[RequireComponent(typeof(GoogleAds))]
public class AdmobExample : MonoBehaviour {
	
	/*
	public bool banner;
	
	public GameObject unitPanel;
	public GameObject rewardedAdButton;
	
	bool startBanner;
	
	GoogleAds ads;
	
	// Use this for initialization
	void Start () {		
		ads = GetComponent<GoogleAds>();
		
		if(banner){
			startBanner = true;
			ads.RequestBanner();
		}
		
		ads.RequestRewardBasedVideo();
	}
	
	void Update(){
		if(startBanner && !Manager.StartMenu.activeSelf){
			startBanner = false;
			ads.bannerView.Destroy();
		}
		
		if(unitPanel.activeSelf && !rewardedAdButton.activeSelf){
			rewardedAdButton.SetActive(true);
		}
		else if(!unitPanel.activeSelf && rewardedAdButton.activeSelf){
			rewardedAdButton.SetActive(false);
		}
	}
	
	public void showRewarded(){
		ads.ShowRewardBasedVideo();
	}
	
	*/
}