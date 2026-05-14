using System.Collections;
using TMPro;
using UnityEngine;

public class OverlayAnimation : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI newChapterText;
    public TextMeshProUGUI objectiveText;

    private CanvasGroup newChapterCanvas;
    private CanvasGroup objectiveCanvas;

    void Awake()
    {
        newChapterCanvas = newChapterText.transform.parent.GetComponent<CanvasGroup>();
        objectiveCanvas = objectiveText.transform.parent.GetComponent<CanvasGroup>();

        newChapterCanvas.alpha = 0f;
        objectiveCanvas.alpha = 0f;

        newChapterText.transform.parent.gameObject.SetActive(false);
        objectiveText.transform.parent.gameObject.SetActive(false);
    }

    public void Play(string chapterTitle, string objectiveHint)
    {
        StopAllCoroutines();

        StartCoroutine(PlaySequence(chapterTitle, objectiveHint));
    }

    IEnumerator PlaySequence(string chapterTitle, string objectiveHint)
    {
        newChapterText.text = chapterTitle;
        objectiveText.text = objectiveHint;

        StartCoroutine(ShowNewChapter());
        StartCoroutine(ShowObjective());

        yield return null;
    }

    IEnumerator ShowNewChapter()
    {
        yield return new WaitForSeconds(1f);

        newChapterText.transform.parent.gameObject.SetActive(true);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;

            newChapterCanvas.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        newChapterCanvas.alpha = 1f;

        yield return new WaitForSeconds(3.5f);

        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;

            newChapterCanvas.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        newChapterCanvas.alpha = 0f;

        newChapterText.transform.parent.gameObject.SetActive(false);
    }

    IEnumerator ShowObjective()
    {
        yield return new WaitForSeconds(2f);

        objectiveText.transform.parent.gameObject.SetActive(true);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;

            objectiveCanvas.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        objectiveCanvas.alpha = 1f;

        yield return new WaitForSeconds(15f);

        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;

            objectiveCanvas.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        objectiveCanvas.alpha = 0f;

        objectiveText.transform.parent.gameObject.SetActive(false);
    }
}