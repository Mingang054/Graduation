using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class RadioTalk : MonoBehaviour
{
    public TMP_Text myText;
    public AudioClip messageSound;

    public List<string> greetTalkList = new();
    public List<string> questTalkList = new();
    public List<string> freeTalkList = new();
    public List<string> shopTalkList = new();
    


    private Dictionary<RadioTalkType, List<string>> talkMap;
    private CancellationTokenSource typingToken;

    private void Awake()
    {
        talkMap = new Dictionary<RadioTalkType, List<string>>
        {
            { RadioTalkType.Greet, greetTalkList },
            { RadioTalkType.Quest, questTalkList },
            { RadioTalkType.Free, freeTalkList },
            { RadioTalkType.Shop, shopTalkList }
        };
    }

    private void OnEnable()
    {
        TryTalk(RadioTalkType.Greet);
    }

    private void OnDisable() => typingToken?.Cancel();
    private void OnDestroy() => typingToken?.Cancel();

    public void TryShopTalk()
    {
        TryTalk(RadioTalkType.Shop);
    }
    public void TryQuestTalk()
    {
        TryTalk(RadioTalkType.Quest);
    }
    public void TryFreeTalk() { TryTalk(RadioTalkType.Free);}

    public void TryTalk(RadioTalkType type, float charDelay = 0.04f)
    {
        if (!talkMap.ContainsKey(type)) return;

        var list = talkMap[type];
        if (list == null || list.Count == 0) return;

        string randomLine = list[UnityEngine.Random.Range(0, list.Count)];
        _ = ShowTextGradually(randomLine, myText, charDelay);
    }

    private async UniTask ShowTextGradually(string text, TMP_Text target, float delay)
    {
        typingToken?.Cancel();
        typingToken = new CancellationTokenSource();
        target.text = "";

        try
        {
            foreach (char c in text)
            {
                target.text += c;
                await UniTask.Delay((int)(delay * 1000), cancellationToken: typingToken.Token);

                if (!char.IsWhiteSpace(c) && messageSound != null)
                    AudioManager.Instance.PlaySFX(messageSound);
            }
        }
        catch (OperationCanceledException) { }
    }
}


public enum RadioTalkType
{
    Greet,
    Quest,
    Free,
    Shop
}
