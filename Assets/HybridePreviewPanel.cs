using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HybridePreviewPanel : BasePanel {

    public BaseButton prevButton;
    public BaseButton nextButton;

    public override void OnClick(BaseButton button)
    {
        if (button == prevButton)
        {
            if (CloudDataManager.Instance.prefabCache.ContainsKey(SharedController.Instance.currentVerandaId - 1))
                ShareManager.Instance.SendSyncMessage(ShareManager.PREVIEWED_VERANDA_CHANGED, SharedController.Instance.currentVerandaId - 1);
            else
            {
                ShareManager.Instance.SendSyncMessage(ShareManager.PREVIEWED_VERANDA_CHANGED, CloudDataManager.Instance.prefabCache.Count);
            }
        }
        else if (button == nextButton)
        {
            if (CloudDataManager.Instance.prefabCache.ContainsKey(SharedController.Instance.currentVerandaId + 1))
                ShareManager.Instance.SendSyncMessage(ShareManager.PREVIEWED_VERANDA_CHANGED, SharedController.Instance.currentVerandaId + 1);
            else
            {
                ShareManager.Instance.SendSyncMessage(ShareManager.PREVIEWED_VERANDA_CHANGED, 1);
            }
        }
    }
}
