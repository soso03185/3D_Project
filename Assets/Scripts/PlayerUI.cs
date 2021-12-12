using UnityEngine;
using Shapes;
using UnityEngine.Rendering;

[ExecuteAlways]
public class PlayerUI : ImmediateModeShapeDrawer
{
	float[] bulletFireTimes;

	[Header("<Count>")]
	public int totalHP = 3;
	public int playerHP = 2;
	public int totalBullets = 20;
	public int bullets = 18;

	[Header("<Draw_AmmoBar>")]
	[SerializeField] float cornerRadius;

	[Header("<Draw_Bullet>")]
	[SerializeField] float bulletThickness = 0.25f;


	void Awake() 
	{
		bulletFireTimes = new float[totalBullets];
		playerHP = totalHP;
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
			DrawHP();
		}
	}
	
	public void DrawBullet()
	{
		if (bullets > totalBullets) bullets = totalBullets;
		if (bullets <= 0) bullets = 0;

		float x = 5.2f;
		float y = -3.1f;
		float alpha = 0.9f;
		float bulletSpace = 0.3f;

		// Draw Bullets
		for (int i = 0; i < bullets; i++)
		{
			Vector2 vecStart = new Vector2(x, y + (i * bulletSpace));
			Vector2 vecEnd = new Vector2(x + 0.8f, y + (i * bulletSpace));

			Draw.Line(vecStart, vecEnd, bulletThickness, new Color(1, 1, 1, alpha));
		}

		// Draw ammoBar
		Draw.RectangleBorder(new Rect(new Vector2(x - 0.3f, y - 1.3f * bulletSpace), new Vector2(1.4f, (totalBullets + 1.5f) * bulletSpace)), 0.1f, cornerRadius);
	}

	public void DrawHP()
    {
		if (playerHP > totalHP) playerHP = totalHP;
		if (playerHP < 0) playerHP = 0;

		float xStart = -6.2f;
		float xEnd = -5.63f;
		float xPos = 2.9f;
		float xSub = xEnd - xStart + 0.3f;

		float yStart = 3.17f;
		float yEnd = 2.6f;
		float yPos = -5.89f;

		for(int i =0; i < playerHP; i ++)
        {
			Draw.Line(new Vector2(xStart, xPos), new Vector2(xEnd, xPos), 0.2f, Color.red);
			Draw.Line(new Vector2(yPos, yStart), new Vector2(yPos, yEnd), 0.2f, Color.red);
        }


    }

}