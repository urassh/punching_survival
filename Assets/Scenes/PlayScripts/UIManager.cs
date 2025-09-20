using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UIイベントを管理する専用のクラス
public class UIManager : MonoBehaviour
{
    // OnClickイベントから、このメソッドを呼び出すように設定する
    public void OnFireButtonClicked()
    {
        Debug.Log("発射ボタンがクリックされました。");

        // PlayerMovement.Local を使って、自分のプレイヤーインスタンスを探す
        // （PlayerCharacter.Localでも同じ考え方です）
        if (PlayerMovement.Local != null)
        {
            // 自分のプレイヤーにアタッチされているBulletSpawnerコンポーネントを取得
            BulletSpawner spawner = PlayerMovement.Local.GetComponent<BulletSpawner>();
            if (spawner != null)
            {
                // そのFire()メソッドを呼び出す
                spawner.Fire();
            }
        }
        else
        {
            Debug.LogError("自分のプレイヤーが見つかりません！ PlayerMovement.Localが設定されていません。");
        }
    }
}
