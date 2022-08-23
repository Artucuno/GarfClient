using UnityEngine;

public class RcSkidMarks : MonoBehaviour
{
	private RcPhysicWheel m_pPhysicWheel;

	private RcVehicle m_pVehicle;

	private float m_fBrakingTimer;

	private float m_fAlpha;

	private float m_fPrevAlpha;

	public int m_iPoolSize;

	public float m_fWidth;

	public float m_fLength;

	public Vector3 m_vOffset;

	public float m_fUVOffset;

	public float m_fDetailFactor;

	private int m_iNbQuads;

	private int m_iNextQuad;

	private bool m_vValidPrevPos;

	private Vector3 m_vPrevPos;

	private Vector3 m_vPrevSide;

	private bool updated;

	private Vector3[] vertices;

	private Vector3[] normals;

	private Vector4[] tangents;

	private Color[] colors;

	private Vector2[] uvs;

	private int[] triangles;

	private MeshFilter meshFilter;

	private MeshRenderer meshRenderer;

	private Mesh m_pMesh;

	public RcSkidMarks()
	{
		m_pVehicle = null;
		m_pPhysicWheel = null;
		m_fBrakingTimer = 0f;
		m_fAlpha = 0f;
		m_iPoolSize = 50;
		m_fWidth = 0.05f;
		m_fLength = 0.4f;
		m_vOffset = new Vector3(0f, 0.05f, 0f);
		m_fDetailFactor = 1f;
		m_vValidPrevPos = false;
		m_vPrevPos = Vector3.zero;
		m_vPrevSide = Vector3.zero;
		m_iNbQuads = 0;
		m_iNextQuad = 0;
		updated = false;
		m_fUVOffset = 0f;
	}

	public void Awake()
	{
		m_pPhysicWheel = GetComponent<RcPhysicWheel>();
		m_pVehicle = base.transform.parent.GetComponentInChildren<RcVehicle>();
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		if (meshFilter == null)
		{
			meshFilter = (MeshFilter)base.gameObject.AddComponent(typeof(MeshFilter));
		}
		if (meshFilter.mesh == null)
		{
			meshFilter.mesh = new Mesh();
		}
		m_pMesh = meshFilter.mesh;
	}

	public void Start()
	{
		m_iNbQuads = m_iPoolSize;
		m_pMesh.Clear();
		m_pMesh.MarkDynamic();
		vertices = new Vector3[m_iPoolSize * 4];
		normals = new Vector3[m_iPoolSize * 4];
		tangents = new Vector4[m_iPoolSize * 4];
		colors = new Color[m_iPoolSize * 4];
		uvs = new Vector2[m_iPoolSize * 4];
		triangles = new int[m_iPoolSize * 6];
		for (int i = 0; i < m_iPoolSize; i++)
		{
			vertices[i * 4] = Vector3.zero;
			vertices[i * 4 + 1] = Vector3.zero;
			vertices[i * 4 + 2] = Vector3.zero;
			vertices[i * 4 + 3] = Vector3.zero;
			normals[i * 4] = Vector3.up;
			normals[i * 4 + 1] = Vector3.up;
			normals[i * 4 + 2] = Vector3.up;
			normals[i * 4 + 3] = Vector3.up;
			tangents[i * 4] = Vector4.zero;
			tangents[i * 4 + 1] = Vector4.zero;
			tangents[i * 4 + 2] = Vector4.zero;
			tangents[i * 4 + 3] = Vector4.zero;
			colors[i * 4] = new Color(0f, 0f, 0f, 0f);
			colors[i * 4 + 1] = new Color(0f, 0f, 0f, 0f);
			colors[i * 4 + 2] = new Color(0f, 0f, 0f, 0f);
			colors[i * 4 + 3] = new Color(0f, 0f, 0f, 0f);
			uvs[i * 4] = new Vector2(0f, 0f);
			uvs[i * 4 + 1] = new Vector2(1f, 0f);
			uvs[i * 4 + 2] = new Vector2(0f, 1f);
			uvs[i * 4 + 3] = new Vector2(1f, 1f);
			triangles[i * 6] = i * 4;
			triangles[i * 6 + 1] = i * 4 + 1;
			triangles[i * 6 + 2] = i * 4 + 2;
			triangles[i * 6 + 3] = i * 4;
			triangles[i * 6 + 4] = i * 4 + 2;
			triangles[i * 6 + 5] = i * 4 + 3;
		}
		m_pMesh.vertices = vertices;
		m_pMesh.normals = normals;
		m_pMesh.tangents = tangents;
		m_pMesh.colors = colors;
		m_pMesh.uv = uvs;
		m_pMesh.triangles = triangles;
	}

	public void LateUpdate()
	{
		base.transform.position = Vector3.zero;
		base.transform.rotation = Quaternion.identity;
		if (base.transform.lossyScale != Vector3.one)
		{
			Vector3 one = Vector3.one;
			one.x = 1f / base.transform.lossyScale.x;
			one.y = 1f / base.transform.lossyScale.y;
			one.z = 1f / base.transform.lossyScale.z;
			base.transform.localScale = one;
		}
		float deltaTime = Time.deltaTime;
		float driftRatio = m_pVehicle.GetDriftRatio();
		if (Mathf.Abs(driftRatio) > 0f && m_pPhysicWheel.BOnGround)
		{
			m_fAlpha = Mathf.Abs(driftRatio);
		}
		else
		{
			m_fAlpha = 0f;
		}
		float motorSpeedMS = m_pVehicle.GetMotorSpeedMS();
		float moveFactor = m_pVehicle.GetMoveFactor();
		if (moveFactor < 0f && motorSpeedMS > 0f)
		{
			m_fBrakingTimer += deltaTime;
		}
		else
		{
			m_fBrakingTimer = 0f;
		}
		if (m_fBrakingTimer > 0.3f)
		{
			float value = m_fBrakingTimer / 1.5f;
			m_fAlpha = Mathf.Max(Mathf.Clamp(value, 0f, 1f), m_fAlpha);
		}
		if (m_pPhysicWheel.FHandBrake * 0.5f > m_fAlpha && m_pPhysicWheel.EAxle == RcPhysicWheel.WheelAxle.Rear)
		{
			m_fAlpha = 0.5f * m_pPhysicWheel.FHandBrake;
		}
		GroundCharac oGroundCharac = m_pPhysicWheel.OGroundCharac;
		Vector3 forward = m_pPhysicWheel.VehicleRoot.parent.forward;
		Vector3 normal = oGroundCharac.normal;
		Vector3 vector = Vector3.Cross(normal, forward);
		forward = Vector3.Cross(vector, normal);
		Vector3 vector2 = m_pPhysicWheel.GetWorldPos() + normal * (m_vOffset.y - m_pPhysicWheel.m_fRadius) + forward * m_vOffset.z;
		if (!m_pPhysicWheel.BOnGround || m_fAlpha <= 0f)
		{
			m_vValidPrevPos = false;
		}
		else
		{
			if (m_pPhysicWheel.ESide == RcPhysicWheel.WheelSide.Left)
			{
				vector2 -= vector * m_vOffset.x;
			}
			else if (m_pPhysicWheel.ESide == RcPhysicWheel.WheelSide.Right)
			{
				vector2 += vector * m_vOffset.x;
			}
			if (!m_vValidPrevPos)
			{
				m_vPrevSide = vector;
				m_vPrevPos = vector2;
			}
			float magnitude = (m_vPrevPos - vector2).magnitude;
			bool reverse = Vector3.Dot(vector2 - m_vPrevPos, forward) < 0f;
			UpdateQuad(normal, m_vPrevPos, vector2, m_vPrevSide, vector, m_fWidth, m_fAlpha, reverse);
			updated = true;
			if (magnitude > m_fLength / m_fDetailFactor)
			{
				NextQuad();
				m_vPrevPos = vector2;
				m_vPrevSide = vector;
			}
			m_vValidPrevPos = true;
		}
		if (updated)
		{
			m_pMesh.Clear();
			m_pMesh.vertices = vertices;
			m_pMesh.normals = normals;
			m_pMesh.tangents = tangents;
			m_pMesh.triangles = triangles;
			m_pMesh.colors = colors;
			m_pMesh.uv = uvs;
			updated = false;
		}
	}

	public void UpdateQuad(Vector3 up, Vector3 previousPos, Vector3 pos, Vector3 previousSide, Vector3 side, float width, float alpha, bool reverse)
	{
		Vector3 lhs = pos - previousPos;
		Vector3 vector = Vector3.Cross(lhs, up);
		vector.Normalize();
		normals[m_iNextQuad * 4] = up;
		normals[m_iNextQuad * 4 + 1] = up;
		normals[m_iNextQuad * 4 + 2] = up;
		normals[m_iNextQuad * 4 + 3] = up;
		tangents[m_iNextQuad * 4] = new Vector4(vector.x, vector.y, vector.z, 1f);
		tangents[m_iNextQuad * 4 + 1] = new Vector4(vector.x, vector.y, vector.z, 1f);
		tangents[m_iNextQuad * 4 + 2] = new Vector4(vector.x, vector.y, vector.z, 1f);
		tangents[m_iNextQuad * 4 + 3] = new Vector4(vector.x, vector.y, vector.z, 1f);
		float num = (pos - previousPos).magnitude / width;
		if (reverse)
		{
			vertices[m_iNextQuad * 4 + 3] = pos - side * width;
			uvs[m_iNextQuad * 4 + 3] = new Vector2(0f, num + m_fUVOffset);
			vertices[m_iNextQuad * 4 + 2] = pos + side * width;
			uvs[m_iNextQuad * 4 + 2] = new Vector2(1f, num + m_fUVOffset);
			vertices[m_iNextQuad * 4 + 1] = previousPos + previousSide * width;
			uvs[m_iNextQuad * 4 + 1] = new Vector2(1f, m_fUVOffset);
			vertices[m_iNextQuad * 4] = previousPos - previousSide * width;
			uvs[m_iNextQuad * 4] = new Vector2(0f, m_fUVOffset);
			colors[m_iNextQuad * 4 + 3] = new Color(0f, 0f, 0f, alpha);
			colors[m_iNextQuad * 4 + 2] = new Color(0f, 0f, 0f, alpha);
			colors[m_iNextQuad * 4 + 1] = new Color(0f, 0f, 0f, m_fPrevAlpha);
			colors[m_iNextQuad * 4] = new Color(0f, 0f, 0f, m_fPrevAlpha);
		}
		else
		{
			vertices[m_iNextQuad * 4] = pos - side * width;
			uvs[m_iNextQuad * 4] = new Vector2(0f, num + m_fUVOffset);
			vertices[m_iNextQuad * 4 + 1] = pos + side * width;
			uvs[m_iNextQuad * 4 + 1] = new Vector2(1f, num + m_fUVOffset);
			vertices[m_iNextQuad * 4 + 2] = previousPos + previousSide * width;
			uvs[m_iNextQuad * 4 + 2] = new Vector2(1f, m_fUVOffset);
			vertices[m_iNextQuad * 4 + 3] = previousPos - previousSide * width;
			uvs[m_iNextQuad * 4 + 3] = new Vector2(0f, m_fUVOffset);
			colors[m_iNextQuad * 4] = new Color(0f, 0f, 0f, alpha);
			colors[m_iNextQuad * 4 + 1] = new Color(0f, 0f, 0f, alpha);
			colors[m_iNextQuad * 4 + 2] = new Color(0f, 0f, 0f, m_fPrevAlpha);
			colors[m_iNextQuad * 4 + 3] = new Color(0f, 0f, 0f, m_fPrevAlpha);
		}
		m_fUVOffset += num;
		m_fUVOffset -= (int)m_fUVOffset;
		m_fPrevAlpha = alpha;
	}

	public void NextQuad()
	{
		m_iNextQuad++;
		if (m_iNextQuad >= m_iNbQuads)
		{
			m_iNextQuad = 0;
		}
	}
}
