
public class Inventory
{
    public int width;  // 인벤토리의 가로 크기
    public int height; // 인벤토리의 세로 크기
    public ItemInstance[,] items; // 아이템 데이터를 포함하는 2D 배열

    public Inventory(int width, int height)
    {
        this.width = width;
        this.height = height;
        items = new ItemInstance[width, height]; // 인벤토리 크기만큼 배열 초기화
    }

    // 아이템 추가
    public bool AddItem(ItemInstance item, int x, int y)
    {
        // 범위 및 충돌 검사 후 아이템 추가
        if (x >= 0 && x < width && y >= 0 && y < height && items[x, y] == null)
        {
            items[x, y] = item;
            return true;
        }
        return false;
    }

    // 아이템 제거
    public void RemoveItem(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            items[x, y] = null;
        }
    }

    // 특정 위치의 아이템 가져오기
    public ItemInstance GetItem(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return items[x, y];
        }
        return null;
    }
}
