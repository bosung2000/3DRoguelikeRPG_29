using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Vector3 offset = new Vector3(0, 7, -10);
    [SerializeField] private Vector3 Rotation = new Vector3(60, 0, 0);
    [SerializeField] private float followSpeed = 0;

    private void LateUpdate()
    {
        if (player == null) return;

        followSpeed = player._playerStat.GetStatValue(PlayerStatType.MoveSpeed);
        Vector3 desiredPosition = player.transform.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Rotation), followSpeed * Time.deltaTime);
    }
    void LookAt()

    {
        // 카메라가 플레이어를 바라보게 하기
        //transform.LookAt(_player);
    }
}