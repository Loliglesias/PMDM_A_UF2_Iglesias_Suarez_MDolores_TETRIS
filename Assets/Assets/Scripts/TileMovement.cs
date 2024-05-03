using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMovement : MonoBehaviour
{
    public float fallSpeed = 2.0f; // Velocidad de caída do bloque
    private bool isFalling = true; // Indica si o bloque está caendo ou non

/*
Actualización constante da animación: 
- Que caia cara abaixo
- Colocalo centrado e dentro do grid
- Asegurarse que non saia do grid ou liña
- Verificar se hai colisións entre tiles e ralentizalos se é que si
*/
    private void Update()
    {
        if (isFalling)
        {
            // O bloque vai cara abaixo sempre
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

            // Asóciase o bloque ás celdas do tilemap
            Vector3Int cellPosition = FindObjectOfType<Tilemap>().WorldToCell(transform.position);

            // Obter a posición no centro da celda
            Vector3 cellCenter = FindObjectOfType<Tilemap>().GetCellCenterWorld(cellPosition);

            // Aliña a posición do tile no centro da celda
            transform.position = new Vector3(cellCenter.x, transform.position.y, transform.position.z);

            // Verificar se hai colisións con outros tiles
            Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, transform.localScale, 0);
            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject != gameObject)
                {
                    // Ralentiza a caída se choca con outro tile
                    isFalling = false;
                    break;
                }
            }
        }
    }
}
