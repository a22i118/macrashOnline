using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MeteorEvent : MonoBehaviour
{
    [SerializeField] private Vector3 _center; // �X�e�[�W�̒��S�ʒu
    [SerializeField] private float _interval; // ���˂̊Ԋu
    private float _accumulate; //�o�ߎ���
    private bool _isFall = false;
    private MeteorPool _meteorPool;
    private MeteorMarkerPool _markerPool;
   [SerializeField] private LayerMask _groundLayer;

    void Start()
    {
        _meteorPool = FindObjectOfType<MeteorPool>();
        _markerPool = FindObjectOfType<MeteorMarkerPool>();
        // �m�F�p�̃R�[�h
        // _isFall = true;
        // StartCoroutine(FallCoroutine());
    }

    //���̃C�x���g�J�n���ɌĂяo�����֐�
    public void Init()
    {
        _isFall = !_isFall;
        if (_isFall)
        {
            StartCoroutine(FallCoroutine());
        }
    }

    private IEnumerator FallCoroutine()
    {
        while (_isFall)
        {
            _accumulate += Time.deltaTime;
            if (_accumulate >= _interval)
            {
                //�o�ߎ��Ԃ������_���ɒZ�k
                _accumulate = Random.Range(0.1f, 1.7f);


                // �����_���Ȑ����ʒu���v�Z
                float _finalposition_x = _center.x + Random.Range(-7.5f, 7.5f);
                float _finalposition_z = _center.z + Random.Range(-4.5f, 4.5f);

                // �}�N�����e�I�ƃ}�[�J�[�̐���
                MakuraMeteor _meteor = _meteorPool.GetGameObject();
                _meteor.transform.position = new Vector3(_finalposition_x, 15f, _finalposition_z);
                MeteorMarker _marker = _markerPool.GetGameObject();
                _marker.MarkerMeteor = _meteor;
                Physics.Raycast(_meteor.transform.position + Vector3.up * 10, Vector3.down, out RaycastHit hit, 30, _groundLayer);
                _marker.transform.position = new Vector3(_finalposition_x, hit.point.y + 0.01f, _finalposition_z);
            }

            yield return null;
        }
    }





}
