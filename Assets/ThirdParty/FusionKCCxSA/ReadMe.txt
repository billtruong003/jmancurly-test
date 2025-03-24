Simple UNITY Multiplayer with PHOTON FUSION ADVANCE KCC
=========================================================
This is a simple example of implementing NetworkMecanimAnimator Photon Fusion using the UNITY Starter Assets Third Person Controller model and animation. You can also use your own custom model later.

1. Getting Photon Fusion Advanced KCC
Sign up if you dont have an account.
Download: https://doc.photonengine.com/fusion/current/technical-samples/fusion-kcc-sample
Extract fusion-kcc-sample-1.1.5.zip
Add Project from UNITY HUB.
Add/Create Photon Fusion App Id in PhotonAppSettings.

2. Import Starter Asset 3rd Person.
Add Asset: https://assetstore.unity.com/packages/essentials/starter-assets-third-person-character-controller-196526
Import Starter Asset 3rd Person from package manager.
We only used for models and animation, so check only ThirdPersonController/Character and import
Note: 
This package is licensed under the Unity Companion License. 
For full license terms, please see: https://unity3d.com/legal/licenses/Unity_Companion_License

3. Import Sample Implementation Package.
Import following custom package to project. with option:

	A. Script Only Package
	If you want to following step-by-step Video or StepToReproduce.txt
	https://drive.google.com/drive/folders/1qvtWrM7VKrTvSTdCPC2s-RFNdaj2bGGA?usp=sharing
	Download: FusionKCCxStarterAssetThirdPerson_ScriptOnly.unitypackage
	Size: 4Kb
	Step to reproduce: StepToReproduce.txt
	Package Content:
		FusionKCCxSA
			ReadMe.txt
			- Scripts
				SimpleAnimator.cs //NetworkBehaviour bridge between Animator and Network Mecanim Animator.
				SimpleFootstep.cs //receiving OnFootstep and OnLand AnimationEvent, usage is optional.

	B. Full Package
	Contain all result, so you can move to step 4 directly.
	https://drive.google.com/drive/folders/1qvtWrM7VKrTvSTdCPC2s-RFNdaj2bGGA?usp=sharing
	Download: FusionKCCxStarterAssetThirdPerson.unitypackage
	Size: 1,1Mb
	Package Content:
		FusionKCCxSA
			ReadMe.txt
			- Prefabs
				- Game
					Gameplay SA
				- Player
					Advanced Player (Third Person) SA
					Armature SA
			- Scenes
				Playground SA
				Village SA
			- Scripts
				SimpleAnimator.cs //NetworkBehaviour bridge between Animator and Network Mecanim Animator.
				SimpleFootstep.cs //receiving OnFootstep and OnLand AnimationEvent, usage is optional.

4. Optional Step 
Edit script in ThirdPersonPlayer.cs for smooth move rotation
				
		// EDITED on line 15
		[SerializeField]
		private float rotationSpeed = 10f; //adjust as needed
		// END
				
		// EDITED on line 168 and 356
        //Visual.Root.rotation = facingRotation;

        Quaternion currentRotation = Visual.Root.rotation;
        Visual.Root.rotation = Quaternion.Slerp(currentRotation, facingRotation, rotationSpeed * Runner.DeltaTime);
		// END

5. Testing
Play Test some Scene from FusionKCCxSA/Scenes.
Simply using parrelsync or any other project instancing to test multiplayer.

parrelsync:
https://github.com/VeriorPies/ParrelSync/releases/download/1.5.1/ParrelSync-1.5.1.unitypackage

DualPlay:
https://assetstore.unity.com/packages/tools/utilities/dualplay-164345

6. Build Testing
Add Scene to build setting.


