using UnityEngine;
using Shapes;
using UnityEngine.Rendering;

[ExecuteAlways]
public class PlayerUI : ImmediateModeShapeDrawer
{
	float[] bulletFireTimes;

    #region Draw_Data

    //Draw Bullet
    float x = 5.3f;
	float y = -3.1f;
	float alpha = 0.9f;
	float bulletSpace = 0.3f;

	//Draw Hp
	float xStart = -6.2f;
	float xEnd = -5.63f;
	float xPos = 2.9f;
	float yStart = 3.17f;
	float yEnd = 2.6f;
	float yPos = -5.89f;

    #endregion

    [Header("<Count>")]
	public int totalBullets = 20;
	public int bullets = 20; 
	public int totalHp = 3;
	public int playerHp = 3;

	[Header("<Draw_AmmoBar>")]
	[SerializeField] float cornerRadius;

	[Header("<Draw_Bullet>")]
	[SerializeField] float bulletThickness = 0.25f;


	void Awake() 
	{
		bulletFireTimes = new float[totalBullets];
		playerHp = totalHp;
	}

	public void Fire() => bulletFireTimes[--bullets] = Time.time;
	public void Reload() => bullets = totalBullets;

	public override void DrawShapes(Camera cam)
	{
		using (Draw.Command(cam))
		{
			Draw.ZTest = CompareFunction.Always;
			Draw.LineGeometry = LineGeometry.Flat2D;
			Draw.Matrix = transform.localToWorldMatrix;

			// bullets
			Draw.LineEndCaps = LineEndCap.Round;
			DrawBullet();

			// Player HP
			DrawHp();
		}
	}
	
	public void DrawBullet()
	{
		if (bullets > totalBullets) bullets = totalBullets;
		if (bullets <= 0) bullets = 0;

		// Draw Bullets
		for (int i = 0; i < bullets; i++)
		{
			Vector2 vecStart = new Vector2(x, y + (i * bulletSpace));
			Vector2 vecEnd = new Vector2(x + 0.5f, y + (i * bulletSpace));

			Draw.Line(vecStart, vecEnd, bulletThickness, new Color(1, 1, 1, alpha));
		}

		// Draw ammoBar
		Draw.RectangleBorder(new Rect(new Vector2(x - 0.3f, y - 1.3f * bulletSpace), new Vector2(1.1f, (totalBullets + 1.5f) * bulletSpace)), 0.1f, cornerRadius);
	}

	public void DrawHp()
    {
		if (playerHp > totalHp) playerHp = totalHp;
		if (playerHp < 0) playerHp = 0;

		Vector2 xSub = new Vector2(xEnd - xStart + 0.3f, 0);
		for (int i =0; i < playerHp; i ++)
        {
			Draw.Line(new Vector2(xStart, xPos) + xSub * i, new Vector2(xEnd, xPos) + xSub * i, 0.2f, new Color(1,0,0, alpha));
			Draw.Line(new Vector2(yPos, yStart) + xSub * i, new Vector2(yPos, yEnd) + xSub * i, 0.2f, new Color(1,0,0, alpha));
        }
	}
}