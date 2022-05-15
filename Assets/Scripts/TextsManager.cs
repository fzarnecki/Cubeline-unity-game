using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextsManager : MonoBehaviour
{
    private string[] texts =
      {
        "Great job! But you can do better..",
        "Great job! But you can do better..",
        "That's it?",
        "That's it?",
        "That's it?",
        "Proud of ya",
        "Just do it!",
        "You are speed",
        "Does our life have any purpose?",
        "You're killin' it",
        "How's your day going?",
        "How's your day going?",
        "How's your day going?",
        "I will show you the way",
        "Just keep going",
        "Just keep going",
        "Just keep going",
        "Nothing to look at..",
        "Nothing to look at..",
        "Nothing to look at..",
        "For real?",
        "For real this time",
        "Stonks",
        "Maybe touch a bit harder",
        "Maybe touch a bit harder",
        "Maybe touch a bit harder",
        "Maybe touch a bit harder",
        "Maybe touch a bit harder",
        "Maybe touch a bit harder",
        "Are you here to look at memes?",
        "Nice one",
        "Just 5 more minutes..",
        "Just 5 more minutes..",
        "Just 5 more minutes..",
        "That's the way to go!",
        "Hear this? Bus driver's clapping",
        "To the moon and beyond..",
        "Almost perfect. Almost",
        "Let's continue, shall we?",
        "Let's continue, shall we?",
        "Let's continue, shall we?",
        "Let's continue, shall we?",
        "Let's continue, shall we?",
        "Lookin' good",
        "Lookin' good",
        "Lookin' good",
        "You beat the game yet?",
        "You beat the game yet?",
        "You beat the game yet?",
        "Alright then, keep your secrets",
        "Let's continue, shall we?",
        "Let's continue, shall we?",
        "Let's continue, shall we?",
        "Your friend has a greater highscore..",
        "Your friend has a greater highscore..",
        "Your friend has a greater highscore..",
        "Your friend has a greater highscore..",
        "Your friend has a greater highscore..",
        "Your friend has a greater highscore..",
        "Almost there",
        "Almost there",
        "Almost there",
        "Almost there",
        "Almost there"
    };

    public string GetRandomText()
    {
        var random = Random.Range(0, texts.Length);
        return texts[random];
    }

    public int GetTextsLength() { return texts.Length; }
}
