
public class Inventory
{
    public int width;  // �κ��丮�� ���� ũ��
    public int height; // �κ��丮�� ���� ũ��
    public ItemInstance[,] items; // ������ �����͸� �����ϴ� 2D �迭

    public Inventory(int width, int height)
    {
        this.width = width;
        this.height = height;
        items = new ItemInstance[width, height]; // �κ��丮 ũ�⸸ŭ �迭 �ʱ�ȭ
    }

    // ������ �߰�
    public bool AddItem(ItemInstance item, int x, int y)
    {
        // ���� �� �浹 �˻� �� ������ �߰�
        if (x >= 0 && x < width && y >= 0 && y < height && items[x, y] == null)
        {
            items[x, y] = item;
            return true;
        }
        return false;
    }

    // ������ ����
    public void RemoveItem(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            items[x, y] = null;
        }
    }

    // Ư�� ��ġ�� ������ ��������
    public ItemInstance GetItem(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return items[x, y];
        }
        return null;
    }
}
