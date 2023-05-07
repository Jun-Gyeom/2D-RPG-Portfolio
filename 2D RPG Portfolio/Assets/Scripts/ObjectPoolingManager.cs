using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectPoolingManager : MonoBehaviour
{
    // 오브젝트 풀링 매니저는 많은 스크립트에서 호출되어야하기 때문에 싱글톤 패턴 사용
    public static ObjectPoolingManager instance; // 싱글톤을 할당할 전역변수

    // 프리팹 변수
    public GameObject enemy_Skull_Prefab;

    public GameObject item_Coin_Prefab;
    public GameObject item_Bullion_Prefab;
    public GameObject item_Exp_Prefab;

    public GameObject hudText_GetGoldText_Prefab;
    public GameObject hudText_GetExpText_Prefab;
    public GameObject hudText_DamageText_Prefab;
    public GameObject hudText_CriticalDamText_Prefab;

    public GameObject equipItem_GraveSword_Prefab;
    public GameObject equipItem_InfernoBlade_Prefab;
    public GameObject equipItem_AmethystAbyssSword_Prefab;
    public GameObject equipItem_SpringShoes_Prefab;
    public GameObject equipItem_WhiteWing_Prefab;
    public GameObject equipItem_ClothArmor_Prefab;
    public GameObject equipItem_ChainArmor_Prefab;
    public GameObject equipItem_TitaniumBreastplate_Prefab;
    public GameObject equipItem_JetPack_Prefab;
    public GameObject equipItem_MetalShoes_Prefab;

    public GameObject bullet_Arrow_Prefab;

    public GameObject effect_ArrowHit_Prefab;

    public GameObject object_SkelHead_Prefab;
    public GameObject object_BoxPiece_Prefab;
    public GameObject object_BigBoxPiece_Prefab;

    // 배열 변수
    GameObject[] enemy_Skull; // 스컬

    GameObject[] item_Coin; // 코인
    GameObject[] item_Bullion; // 금괴
    GameObject[] item_Exp; // 경험치

    GameObject[] hudText_GetGoldText; // 골드 획득 허드텍스트
    GameObject[] hudText_GetExpText; // 경험치 획득 허드텍스트
    GameObject[] hudText_DamageText; // 대미지 허드텍스트
    GameObject[] hudText_CriticalDamText; // 크리티컬 대미지 허드텍스트

    GameObject[] equipItem_GraveSword; // 그레이브 소드 아이템
    GameObject[] equipItem_InfernoBlade; // 인페르노 블레이드 아이템
    GameObject[] equipItem_AmethystAbyssSword; // 자수정 심연 검 아이템
    GameObject[] equipItem_SpringShoes; // 스프링 신발 아이템
    GameObject[] equipItem_WhiteWing; // 하얀 날개 아이템
    GameObject[] equipItem_ClothArmor; // 천 갑옷 아이템
    GameObject[] equipItem_ChainArmor; // 사슬 갑옷 아이템
    GameObject[] equipItem_TitaniumBreastplate; // 티타늄 흉갑 아이템
    GameObject[] equipItem_JetPack; // 제트팩 아이템
    GameObject[] equipItem_MetalShoes; // 강철 신발 아이템

    GameObject[] bullet_Arrow; // 화살 발사체

    GameObject[] effect_ArrowHit; // 화살 충돌 이펙트

    GameObject[] object_SkelHead; // 스켈 머리 오브젝트
    GameObject[] object_BoxPiece; // 박스 파편 오브젝트
    GameObject[] object_BigBoxPiece; // 큰 박스 파편 오브젝트

    GameObject[] targetPool; // 오브젝트 종류 구별을 위한 변수

    private void Awake()
    {


        // 싱글톤 패턴
        if (instance == null) // 싱글톤 변수가 비어있으면
        {
            instance = this; // 자신을 할당
        }
        else // 싱글톤 변수가 비어있지 않으면
        {
            Destroy(gameObject); // 자신을 파괴
        }
        DontDestroyOnLoad(gameObject); // 씬 전환 시에도 ObjectPoolingManager가 삭제되지 않음

        // 이벤트 등록
        SceneManager.sceneLoaded += LoadedsceneEvent;

        enemy_Skull = new GameObject[50];

        item_Coin = new GameObject[300];
        item_Bullion = new GameObject[100];
        item_Exp = new GameObject[300];

        hudText_GetGoldText = new GameObject[50];
        hudText_GetExpText = new GameObject[50];
        hudText_DamageText = new GameObject[50];
        hudText_CriticalDamText = new GameObject[50];

        equipItem_GraveSword = new GameObject[10];
        equipItem_InfernoBlade = new GameObject[10];
        equipItem_AmethystAbyssSword = new GameObject[10];
        equipItem_SpringShoes = new GameObject[10];
        equipItem_WhiteWing = new GameObject[10];
        equipItem_ClothArmor = new GameObject[10];
        equipItem_ChainArmor = new GameObject[10];
        equipItem_TitaniumBreastplate = new GameObject[10];
        equipItem_MetalShoes = new GameObject[10];
        equipItem_JetPack = new GameObject[10];

        bullet_Arrow = new GameObject[50];

        effect_ArrowHit = new GameObject[50];

        object_SkelHead = new GameObject[50];
        object_BoxPiece = new GameObject[50];
        object_BigBoxPiece = new GameObject[50];

        Generate(); // 인스턴스 생성
    }

    // 인스턴스 생성 함수
    void Generate()
    {
        // 배열의 길이만큼 오브젝트를 미리 생성
        
        // 스컬 몬스터
        for (int i = 0; i < enemy_Skull.Length; i++)
        {
            enemy_Skull[i] = Instantiate(enemy_Skull_Prefab); // 인스턴스 생성
            enemy_Skull[i].SetActive(false); // 생성 후 바로 비활성화
            DontDestroyOnLoad(enemy_Skull[i]); // 씬 변경 시에도 삭제되지 않음
        }

        // 코인
        for (int i = 0; i < item_Coin.Length; i++)
        {
            item_Coin[i] = Instantiate(item_Coin_Prefab); // 인스턴스 생성
            item_Coin[i].SetActive(false); // 생성 후 바로 비활성화
            DontDestroyOnLoad(item_Coin[i]); // 씬 변경 시에도 삭제되지 않음
        }

        // 금괴
        for (int i = 0; i < item_Bullion.Length; i++)
        {
            item_Bullion[i] = Instantiate(item_Bullion_Prefab); // 인스턴스 생성
            item_Bullion[i].SetActive(false); // 생성 후 바로 비활성화
            DontDestroyOnLoad(item_Bullion[i]); // 씬 변경 시에도 삭제되지 않음
        }

        // 경험치
        for (int i = 0; i < item_Exp.Length; i++)
        {
            item_Exp[i] = Instantiate(item_Exp_Prefab); // 인스턴스 생성
            item_Exp[i].SetActive(false); // 생성 후 바로 비활성화
            DontDestroyOnLoad(item_Exp[i]); // 씬 변경 시에도 삭제되지 않음
        }

        // 골드 획득 허드텍스트
        for (int i = 0; i < hudText_GetGoldText.Length; i++)
        {
            hudText_GetGoldText[i] = Instantiate(hudText_GetGoldText_Prefab); // 인스턴스 생성
            hudText_GetGoldText[i].SetActive(false); // 생성 후 바로 비활성화
            DontDestroyOnLoad(hudText_GetGoldText[i]); // 씬 변경 시에도 삭제되지 않음
        }

        // 경험치 획득 허드텍스트
        for (int i = 0; i < hudText_GetExpText.Length; i++)
        {
            hudText_GetExpText[i] = Instantiate(hudText_GetExpText_Prefab); // 인스턴스 생성
            hudText_GetExpText[i].SetActive(false); // 생성 후 바로 비활성화
            DontDestroyOnLoad(hudText_GetExpText[i]); // 씬 변경 시에도 삭제되지 않음
        }

        // 대미지 허드텍스트
        for (int i = 0; i < hudText_DamageText.Length; i++)
        {
            hudText_DamageText[i] = Instantiate(hudText_DamageText_Prefab); // 인스턴스 생성
            hudText_DamageText[i].SetActive(false); // 생성 후 바로 비활성화
            DontDestroyOnLoad(hudText_DamageText[i]); // 씬 변경 시에도 삭제되지 않음
        }

        // 크리티컬 대미지 허드텍스트
        for (int i = 0; i < hudText_CriticalDamText.Length; i++)
        {
            hudText_CriticalDamText[i] = Instantiate(hudText_CriticalDamText_Prefab); // 인스턴스 생성
            hudText_CriticalDamText[i].SetActive(false); // 생성 후 바로 비활성화
            DontDestroyOnLoad(hudText_CriticalDamText[i]); // 씬 변경 시에도 삭제되지 않음
        }

        // 그레이브 소드 아이템
        for (int i = 0; i < equipItem_GraveSword.Length; i++)
        {
            equipItem_GraveSword[i] = Instantiate(equipItem_GraveSword_Prefab); // 인스턴스 생성
            equipItem_GraveSword[i].SetActive(false); // 생성 후 바로 비활성화
            DontDestroyOnLoad(equipItem_GraveSword[i]); // 씬 변경 시에도 삭제되지 않음
        }

        // 인페르노 블레이드 아이템
        for (int i = 0; i < equipItem_InfernoBlade.Length; i++)
        {
            equipItem_InfernoBlade[i] = Instantiate(equipItem_InfernoBlade_Prefab); // 인스턴스 생성
            equipItem_InfernoBlade[i].SetActive(false); // 생성 후 바로 비활성화
            DontDestroyOnLoad(equipItem_InfernoBlade[i]); // 씬 변경 시에도 삭제되지 않음
        }

        // 자수정 심연 검 아이템
        for (int i = 0; i < equipItem_AmethystAbyssSword.Length; i++)
        {
            equipItem_AmethystAbyssSword[i] = Instantiate(equipItem_AmethystAbyssSword_Prefab); // 인스턴스 생성
            equipItem_AmethystAbyssSword[i].SetActive(false); // 생성 후 바로 비활성화
            DontDestroyOnLoad(equipItem_AmethystAbyssSword[i]); // 씬 변경 시에도 삭제되지 않음
        }

        // 스프링 신발 아이템
        for (int i = 0; i < equipItem_SpringShoes.Length; i++)
        {
            equipItem_SpringShoes[i] = Instantiate(equipItem_SpringShoes_Prefab); // 인스턴스 생성
            equipItem_SpringShoes[i].SetActive(false); // 생성 후 바로 비활성화
            DontDestroyOnLoad(equipItem_SpringShoes[i]); // 씬 변경 시에도 삭제되지 않음
        }

        // 하얀 날개 아이템
        for (int i = 0; i < equipItem_WhiteWing.Length; i++)
        {
            equipItem_WhiteWing[i] = Instantiate(equipItem_WhiteWing_Prefab); // 인스턴스 생성
            equipItem_WhiteWing[i].SetActive(false); // 생성 후 바로 비활성화
            DontDestroyOnLoad(equipItem_WhiteWing[i]); // 씬 변경 시에도 삭제되지 않음
        }

        // 천 갑옷 아이템
        for (int i = 0; i < equipItem_ClothArmor.Length; i++)
        {
            equipItem_ClothArmor[i] = Instantiate(equipItem_ClothArmor_Prefab); // 인스턴스 생성
            equipItem_ClothArmor[i].SetActive(false); // 생성 후 바로 비활성화
            DontDestroyOnLoad(equipItem_ClothArmor[i]); // 씬 변경 시에도 삭제되지 않음
        }

        // 사슬 갑옷 아이템
        for (int i = 0; i < equipItem_ChainArmor.Length; i++)
        {
            equipItem_ChainArmor[i] = Instantiate(equipItem_ChainArmor_Prefab); // 인스턴스 생성
            equipItem_ChainArmor[i].SetActive(false); // 생성 후 바로 비활성화
            DontDestroyOnLoad(equipItem_ChainArmor[i]); // 씬 변경 시에도 삭제되지 않음
        }

        // 티타늄 흉갑 아이템
        for (int i = 0; i < equipItem_TitaniumBreastplate.Length; i++)
        {
            equipItem_TitaniumBreastplate[i] = Instantiate(equipItem_TitaniumBreastplate_Prefab); // 인스턴스 생성
            equipItem_TitaniumBreastplate[i].SetActive(false); // 생성 후 바로 비활성화
            DontDestroyOnLoad(equipItem_TitaniumBreastplate[i]); // 씬 변경 시에도 삭제되지 않음
        }

        // 제트팩 아이템
        for (int i = 0; i < equipItem_JetPack.Length; i++)
        {
            equipItem_JetPack[i] = Instantiate(equipItem_JetPack_Prefab); // 인스턴스 생성
            equipItem_JetPack[i].SetActive(false); // 생성 후 바로 비활성화
            DontDestroyOnLoad(equipItem_JetPack[i]); // 씬 변경 시에도 삭제되지 않음
        }

        // 강철 신발 아이템
        for (int i = 0; i < equipItem_MetalShoes.Length; i++)
        {
            equipItem_MetalShoes[i] = Instantiate(equipItem_MetalShoes_Prefab); // 인스턴스 생성
            equipItem_MetalShoes[i].SetActive(false); // 생성 후 바로 비활성화
            DontDestroyOnLoad(equipItem_MetalShoes[i]); // 씬 변경 시에도 삭제되지 않음
        }

        // 화살 발사체
        for (int i = 0; i < bullet_Arrow.Length; i++)
        {
            bullet_Arrow[i] = Instantiate(bullet_Arrow_Prefab); // 인스턴스 생성
            bullet_Arrow[i].SetActive(false); // 생성 후 바로 비활성화
            DontDestroyOnLoad(bullet_Arrow[i]); // 씬 변경 시에도 삭제되지 않음
        }

        // 화살 충돌 이펙트
        for (int i = 0; i < effect_ArrowHit.Length; i++)
        {
            effect_ArrowHit[i] = Instantiate(effect_ArrowHit_Prefab); // 인스턴스 생성
            effect_ArrowHit[i].SetActive(false); // 생성 후 바로 비활성화
            DontDestroyOnLoad(effect_ArrowHit[i]); // 씬 변경 시에도 삭제되지 않음
        }

        // 스켈 머리 오브젝트
        for (int i = 0; i < object_SkelHead.Length; i++)
        {
            object_SkelHead[i] = Instantiate(object_SkelHead_Prefab); // 인스턴스 생성
            object_SkelHead[i].SetActive(false); // 생성 후 바로 비활성화
            DontDestroyOnLoad(object_SkelHead[i]); // 씬 변경 시에도 삭제되지 않음
        }

        // 박스 파편 오브젝트
        for (int i = 0; i < object_BoxPiece.Length; i++)
        {
            object_BoxPiece[i] = Instantiate(object_BoxPiece_Prefab); // 인스턴스 생성
            object_BoxPiece[i].SetActive(false); // 생성 후 바로 비활성화
            DontDestroyOnLoad(object_BoxPiece[i]); // 씬 변경 시에도 삭제되지 않음
        }

        // 큰 박스 파편 오브젝트
        for (int i = 0; i < object_BigBoxPiece.Length; i++)
        {
            object_BigBoxPiece[i] = Instantiate(object_BigBoxPiece_Prefab); // 인스턴스 생성
            object_BigBoxPiece[i].SetActive(false); // 생성 후 바로 비활성화
            DontDestroyOnLoad(object_BigBoxPiece[i]); // 씬 변경 시에도 삭제되지 않음
        }
    }

    public GameObject GetObject(string type)
    {
        // 스위치 문을 통해 원하는 타입의 오브젝트 풀을 선택
        switch (type)
        {
            case "Enemy_Skull":
                targetPool = enemy_Skull;
                break;
            case "Item_Coin":
                targetPool = item_Coin;
                break;
            case "Item_Bullion":
                targetPool = item_Bullion;
                break;
            case "Item_Exp":
                targetPool = item_Exp;
                break;
            case "HudText_GetGold":
                targetPool = hudText_GetGoldText;
                break;
            case "HudText_GetExp":
                targetPool = hudText_GetExpText;
                break;
            case "HudText_Damage":
                targetPool = hudText_DamageText;
                break;
            case "HudText_CriticalDam":
                targetPool = hudText_CriticalDamText;
                break;
            case "EquipItem_GraveSword":
                targetPool = equipItem_GraveSword;
                break;
            case "EquipItem_InfernoBlade":
                targetPool = equipItem_InfernoBlade;
                break;
            case "EquipItem_AmethystAbyssSword":
                targetPool = equipItem_AmethystAbyssSword;
                break;
            case "EquipItem_SpringShoes":
                targetPool = equipItem_SpringShoes;
                break;
            case "EquipItem_WhiteWing":
                targetPool = equipItem_WhiteWing;
                break;
            case "EquipItem_ClothArmor":
                targetPool = equipItem_ClothArmor;
                break;
            case "EquipItem_ChainArmor":
                targetPool = equipItem_ChainArmor;
                break;
            case "EquipItem_TitaniumBreastplate":
                targetPool = equipItem_TitaniumBreastplate;
                break;
            case "EquipItem_JetPack":
                targetPool = equipItem_JetPack;
                break;
            case "EquipItem_MetalShoes":
                targetPool = equipItem_MetalShoes;
                break;
            case "Bullet_Arrow":
                targetPool = bullet_Arrow;
                break;
            case "Effect_ArrowHit":
                targetPool = effect_ArrowHit;
                break;
            case "Object_SkelHead":
                targetPool = object_SkelHead;
                break;
            case "Object_BoxPiece":
                targetPool = object_BoxPiece;
                break;
            case "Object_BigBoxPiece":
                targetPool = object_BigBoxPiece;
                break;
        }

        // 선택한 오브젝트 풀을 뒤져서 비활성화 된(사용 중이지 않은) 오브젝트를 찾음
        for (int i = 0; i < targetPool.Length; i++)
        {
            // 비활성화 된 오브젝트를 찾았다면
            if (!targetPool[i].activeSelf)
            {
                targetPool[i].SetActive(true); // 오브젝트 활성화
                return targetPool[i]; // 오브젝트 대여
            }
        }

        // 모종의 이유로 오브젝트를 찾지 못했다면
        return null; // 아무것도 반환하지 않음
    }

    // 씬 변경 시 발동 이벤트
    private void LoadedsceneEvent(Scene arg0, LoadSceneMode arg1)
    {
        // 모든 오브젝트 풀을 뒤져서 활성화 된 오브젝트를 찾음

        // 코인
        for (int i = 0; i < item_Coin.Length; i++)
        {
            // 활성화 된 오브젝트를 찾았다면
            if (item_Coin[i].activeSelf)
            {
                item_Coin[i].SetActive(false); // 오브젝트 비활성화
            }
        }

        // 경험치
        for (int i = 0; i < item_Exp.Length; i++)
        {
            // 활성화 된 오브젝트를 찾았다면
            if (item_Exp[i].activeSelf)
            {
                item_Exp[i].SetActive(false); // 오브젝트 비활성화
            }
        }

        // 금괴
        for (int i = 0; i < item_Bullion.Length; i++)
        {
            // 활성화 된 오브젝트를 찾았다면
            if (item_Bullion[i].activeSelf)
            {
                item_Bullion[i].SetActive(false); // 오브젝트 비활성화
            }
        }

        // 그레이브 소드
        for (int i = 0; i < equipItem_GraveSword.Length; i++)
        {
            // 활성화 된 오브젝트를 찾았다면
            if (equipItem_GraveSword[i].activeSelf)
            {
                equipItem_GraveSword[i].SetActive(false); // 오브젝트 비활성화
            }
        }

        // 인페르노 블레이드
        for (int i = 0; i < equipItem_InfernoBlade.Length; i++)
        {
            // 활성화 된 오브젝트를 찾았다면
            if (equipItem_InfernoBlade[i].activeSelf)
            {
                equipItem_InfernoBlade[i].SetActive(false); // 오브젝트 비활성화
            }
        }

        // 자수정 심연 검
        for (int i = 0; i < equipItem_AmethystAbyssSword.Length; i++)
        {
            // 활성화 된 오브젝트를 찾았다면
            if (equipItem_AmethystAbyssSword[i].activeSelf)
            {
                equipItem_AmethystAbyssSword[i].SetActive(false); // 오브젝트 비활성화
            }
        }

        // 스프링 신발
        for (int i = 0; i < equipItem_SpringShoes.Length; i++)
        {
            // 활성화 된 오브젝트를 찾았다면
            if (equipItem_SpringShoes[i].activeSelf)
            {
                equipItem_SpringShoes[i].SetActive(false); // 오브젝트 비활성화
            }
        }

        // 하얀 날개
        for (int i = 0; i < equipItem_WhiteWing.Length; i++)
        {
            // 활성화 된 오브젝트를 찾았다면
            if (equipItem_WhiteWing[i].activeSelf)
            {
                equipItem_WhiteWing[i].SetActive(false); // 오브젝트 비활성화
            }
        }

        // 천 갑옷
        for (int i = 0; i < equipItem_ClothArmor.Length; i++)
        {
            // 활성화 된 오브젝트를 찾았다면
            if (equipItem_ClothArmor[i].activeSelf)
            {
                equipItem_ClothArmor[i].SetActive(false); // 오브젝트 비활성화
            }
        }

        // 사슬 갑옷
        for (int i = 0; i < equipItem_ChainArmor.Length; i++)
        {
            // 활성화 된 오브젝트를 찾았다면
            if (equipItem_ChainArmor[i].activeSelf)
            {
                equipItem_ChainArmor[i].SetActive(false); // 오브젝트 비활성화
            }
        }

        // 티타늄 흉갑
        for (int i = 0; i < equipItem_TitaniumBreastplate.Length; i++)
        {
            // 활성화 된 오브젝트를 찾았다면
            if (equipItem_TitaniumBreastplate[i].activeSelf)
            {
                equipItem_TitaniumBreastplate[i].SetActive(false); // 오브젝트 비활성화
            }
        }

        // 제트팩
        for (int i = 0; i < equipItem_JetPack.Length; i++)
        {
            // 활성화 된 오브젝트를 찾았다면
            if (equipItem_JetPack[i].activeSelf)
            {
                equipItem_JetPack[i].SetActive(false); // 오브젝트 비활성화
            }
        }

        // 강철 신발
        for (int i = 0; i < equipItem_MetalShoes.Length; i++)
        {
            // 활성화 된 오브젝트를 찾았다면
            if (equipItem_MetalShoes[i].activeSelf)
            {
                equipItem_MetalShoes[i].SetActive(false); // 오브젝트 비활성화
            }
        }

        // 화살 발사체
        for (int i = 0; i < bullet_Arrow.Length; i++)
        {
            // 활성화 된 오브젝트를 찾았다면
            if (bullet_Arrow[i].activeSelf)
            {
                bullet_Arrow[i].SetActive(false); // 오브젝트 비활성화
            }
        }

        // 스켈 머리 오브젝트
        for (int i = 0; i < object_SkelHead.Length; i++)
        {
            // 활성화 된 오브젝트를 찾았다면
            if (object_SkelHead[i].activeSelf)
            {
                object_SkelHead[i].SetActive(false); // 오브젝트 비활성화
            }
        }

        // 박스 파편 오브젝트
        for (int i = 0; i < object_BigBoxPiece.Length; i++)
        {
            // 활성화 된 오브젝트를 찾았다면
            if (object_BigBoxPiece[i].activeSelf)
            {
                object_BigBoxPiece[i].SetActive(false); // 오브젝트 비활성화
            }
        }

        // 큰 박스 파편 오브젝트
        for (int i = 0; i < object_BigBoxPiece.Length; i++)
        {
            // 활성화 된 오브젝트를 찾았다면
            if (object_BigBoxPiece[i].activeSelf)
            {
                object_BigBoxPiece[i].SetActive(false); // 오브젝트 비활성화
            }
        }
    }
}
