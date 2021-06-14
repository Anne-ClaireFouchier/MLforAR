# Deep learning in Unity
## Barracuda 

Barracuda ([documentation](https://docs.unity3d.com/Packages/com.unity.barracuda@1.0/manual/index.html)) is a lightweight cross-platform library for neural network inference.

[Unity_Detection2AR](https://github.com/derenlei/Unity_Detection2AR) is a great (and one a very few) example for using Barracuda. 

They make use of [arfoundation-samples](https://github.com/Unity-Technologies/arfoundation-samples), which demonstrates the functionnalities of the ARFoundation package, allowing for AR functionnalities.


Here is how deep learning networks are added into Unity through Barracuda : 
<img src="https://user-images.githubusercontent.com/61414950/121860532-3a2eca00-ccf9-11eb-8140-7e2fbb556454.png" height="800">

The model needs to be converted into ONNX or Barracuda format.

Instructions [here](https://docs.unity3d.com/Packages/com.unity.barracuda@1.0/manual/Exporting.html) to convert from Pytorch, TensorFlow or Keras to ONNX. 

Instructions [here](https://docs.unity3d.com/Packages/com.unity.barracuda@0.3/manual/index.html) to convert TensorFlow models to Barracuda format.


If the network has the right format, it will appear as follow : 
<img width="1676" alt="Capture d’écran 2021-06-14 à 11 58 11" src="https://user-images.githubusercontent.com/61414950/121877727-386e0200-cd0b-11eb-9138-0b084429c738.png">





The detection happens in the script *PhoneARCamera* 
<img width="1680" alt="Capture d’écran 2021-06-14 à 12 11 47" src="https://user-images.githubusercontent.com/61414950/121882145-70c40f00-cd10-11eb-80a9-eaa978888bab.png">




## Plugins

With the following Plugins, I was able to run the examples but I was not able to integrate ARFoundation in order to put AR augmentations with [ARFoundation](https://github.com/Unity-Technologies/arfoundation-samples) to the scenes. 

### Mediapipe plugin
[MediaPipeUnityPlugin](https://github.com/homuler/MediaPipeUnityPlugin) implements the following [Mediapipe](https://google.github.io/mediapipe/) functionnalities : 

+ Face Detection
+ Face Mesh
+ Iris
+ Hands
+ Pose
+ Holistic (with iris)
+ Hair Segmentation
+ Object Detection
+ Box Tracking
+ Instant Motion Tracking
+ Objectron	

It does not recognize my camera on the phone and I wan not able to add any AR functionalities. 

### TensorFlow Plugin

[tf-lite-unity-sample](https://github.com/asus4/tf-lite-unity-sample) the following functionalities into Unity :

+ TensorFlow
  * MNIST
  * SSD Object Detection
  * DeepLab
  * PoseNet
  * Style Transfer
  * Text Classification
  * Bert Question and Answer
  * Super Resolution
+ MediaPipe
  * Hand Tracking
  * Blaze Face
  * Face Mesh
  * Blaze Pose (Upper body)
+ MLKit
  * Blaze Pose (Full body)
+ Meet Segmentation



# Deep learning with Android Studio

## AR Core Android Studio

[Documentation](https://developers.google.com/ar/develop/java/quickstart)

[API](https://github.com/google-ar/arcore-android-sdk)







# Depth estimation

## MobilePyDnet
[MobilePyDnet](https://github.com/FilippoAleotti/mobilePydnet) based on the [PyDnet](https://github.com/mattpoggi/pydnet) architecture, trained with [knowledge distillation](https://arxiv.org/abs/1503.02531) of [MiDaS](https://github.com/intel-isl/MiDaS)



![distillation](https://user-images.githubusercontent.com/61414950/121861238-f38d9f80-ccf9-11eb-8435-38e43cf6228b.png)


## FastDepth
[FastDepth](http://fastdepth.mit.edu)



# Distortions
[Data_augmentation/Distortions.ipynb](Data_augmentation/Distortions.ipynb) gathers distortions from [CollabAR](https://github.com/CollabAR-Source/CollabAR-Code) [(paper)](https://ieeexplore.ieee.org/abstract/document/9111024) and [imgaug.augmenters](https://imgaug.readthedocs.io/en/latest/source/overview/imgcorruptlike.html).


# More links on depth




