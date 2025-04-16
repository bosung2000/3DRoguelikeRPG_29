using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Vector3 offset = new Vector3(0, 15, -10);
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private PlayerStat _playerStat;

    private void LateUpdate()
    {
        if (_player == null) return;

        Vector3 desiredPosition = _player.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        followSpeed = _playerStat.GetStatValue(PlayerStatType.Speed);
    }
    void LookAt()

    {
        // 카메라가 플레이어를 바라보게 하기
        //transform.LookAt(_player);
    }
}