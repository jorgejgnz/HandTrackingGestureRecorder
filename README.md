# Industrial Tech Demo (Oculus Quest)

Unity utility to help devs to record hand tracking gestures easier

![Recording hand tracking gestures](https://i.imgur.com/gAzxJx1.gif)

## Getting Started

How can I use this?

1. Download GestureRecognizer.cs
2. Move it to your project folder
3. Make custom gameobjects to follow each finger tip as well as the base of the hand
4. Attach GestureRecognizer component to any object in the Scene
5. Set Hand with the gameobject taht follows base of the Hand
6. Set the array of gameobjects with the gameobjects that follow each finger tip
7. Connect your Oculus Quest with Oculus Link

8. Connect and wake up your Oculus Quest
9. Start Oculus Link

10. Start Play Mode in Unity
11. Make the desired gesture with your hand
12. Pause Play Mode
13. Press "Save current gesture" in Inspector
14. Copy the GestureRecognizer component
15. Stop Play Mode
16. Paste Component Values in the same GestureRecognizer component

17. Deploy the new Gesture object added to the list of Saved Gestures
18. Fill the onRecognize event to perform desired actions when gesture is detected

## Built With

This Unity project uses the following:

* [Unity 2019.2.9f1](https://unity3d.com/es/get-unity/download/archive)
* [Oculus Integration 13.0](https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022)
* Oculus Link needed

This script can be used without Oculus Integration as it can record and recognize any configuration of positions in a tree of objects.

## Author

* **Jorge Juan González** - *HCI Researcher at I3A (UCLM)* - [GitHub](https://github.com/jormaje) - [LinkedIn](https://www.linkedin.com/in/jorgejgnz/) - [ResearchGate](https://www.researchgate.net/profile/Jorge_Juan_Gonzalez)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
