using UnityEngine;
using UnityEngine.UI;

public class BackgroundScroller : MonoBehaviour
{
    [Header("스크롤 설정")]
    public Vector2 scrollSpeed = new Vector2(0.5f, 0.5f); // X, Y 스크롤 속도
    public bool scrollHorizontal = true; // 수평 스크롤 활성화
    public bool scrollVertical = true;   // 수직 스크롤 활성화
    public bool reverseHorizontal = false; // 수평 스크롤 방향 반전
    public bool reverseVertical = false;   // 수직 스크롤 방향 반전
    
    [Header("래핑 설정")]
    public bool wrapHorizontal = true;  // 수평 래핑 (순환) 활성화
    public bool wrapVertical = true;    // 수직 래핑 (순환) 활성화
    
    // 내부 변수
    private RawImage rawImage;      // RawImage UI 컴포넌트
    private Image image;            // Image UI 컴포넌트
    private Renderer spriteRenderer; // 스프라이트 렌더러인 경우
    private Material material;      // 머티리얼 참조
    private enum ScrollerType { RawImage, Image, Renderer, None }
    private ScrollerType scrollerType = ScrollerType.None;
    
    private void Start()
    {
        // 컴포넌트 확인 및 참조 설정
        rawImage = GetComponent<RawImage>();
        if (rawImage != null)
        {
            scrollerType = ScrollerType.RawImage;
            return;
        }
        
        image = GetComponent<Image>();
        if (image != null)
        {
            // Image 컴포넌트는 Material 속성을 사용하여 머티리얼을 가져옴
            if (image.material == null)
            {
                // 기본 UI 머티리얼을 복제하여 사용
                material = new Material(Shader.Find("UI/Default"));
            }
            else
            {
                material = new Material(image.material);
            }
            image.material = material;
            scrollerType = ScrollerType.Image;
            return;
        }
        
        spriteRenderer = GetComponent<Renderer>();
        if (spriteRenderer != null)
        {
            // 공유 머티리얼이 수정되지 않도록 인스턴스 생성
            material = spriteRenderer.material;
            scrollerType = ScrollerType.Renderer;
            return;
        }
        
        // 적합한 컴포넌트를 찾지 못한 경우
        Debug.LogError("BackgroundScroller: 이 게임오브젝트에 RawImage, Image 또는 Renderer가 없습니다!");
        enabled = false;
    }
    
    private void Update()
    {
        // 스크롤 방향 설정
        float scrollX = scrollHorizontal ? (reverseHorizontal ? -1 : 1) * scrollSpeed.x * Time.deltaTime : 0;
        float scrollY = scrollVertical ? (reverseVertical ? -1 : 1) * scrollSpeed.y * Time.deltaTime : 0;
        
        switch (scrollerType)
        {
            case ScrollerType.RawImage:
                ScrollRawImage(scrollX, scrollY);
                break;
                
            case ScrollerType.Image:
            case ScrollerType.Renderer:
                ScrollMaterial(scrollX, scrollY);
                break;
        }
    }
    
    private void ScrollRawImage(float scrollX, float scrollY)
    {
        // 현재 UV 좌표 가져오기
        Rect uvRect = rawImage.uvRect;
        
        // UV 좌표 이동
        uvRect.x += scrollX;
        uvRect.y += scrollY;
        
        // 래핑 처리 (0~1 범위 유지)
        if (wrapHorizontal)
        {
            uvRect.x = Mathf.Repeat(uvRect.x, 1.0f);
        }
        
        if (wrapVertical)
        {
            uvRect.y = Mathf.Repeat(uvRect.y, 1.0f);
        }
        
        // 변경된 UV 좌표 적용
        rawImage.uvRect = uvRect;
    }
    
    private void ScrollMaterial(float scrollX, float scrollY)
    {
        if (material == null) return;
        
        // 현재 오프셋 가져오기
        Vector2 offset = material.mainTextureOffset;
        
        // 오프셋 이동
        offset.x += scrollX;
        offset.y += scrollY;
        
        // 래핑 처리 (0~1 범위 유지)
        if (wrapHorizontal)
        {
            offset.x = Mathf.Repeat(offset.x, 1.0f);
        }
        
        if (wrapVertical)
        {
            offset.y = Mathf.Repeat(offset.y, 1.0f);
        }
        
        // 변경된 오프셋 적용
        material.mainTextureOffset = offset;
    }
    
    // 스크롤 속도 동적 변경 메서드
    public void SetScrollSpeed(Vector2 newSpeed)
    {
        scrollSpeed = newSpeed;
    }
    
    // 스크롤 방향 동적 변경 메서드
    public void SetScrollDirection(bool horizontal, bool vertical, bool reverseH = false, bool reverseV = false)
    {
        scrollHorizontal = horizontal;
        scrollVertical = vertical;
        reverseHorizontal = reverseH;
        reverseVertical = reverseV;
    }
}
