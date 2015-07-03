using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CADCAM
{
    /// <summary>
    /// Base class for representing single tringle
    /// </summary>
    public struct Triangle: IVertexType
    {
        Vector3 _vertexPosition;
        Vector2 _vertexTextureCoordinate;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );

        //The constructor for the custom vertex. This allows similar 
        //initialization of custom vertex arrays as compared to arrays of a 
        //standard vertex type, such as VertexPositionColor.
        public Triangle(Vector3 pos, Vector2 textureCoordinate)
        {
            _vertexPosition = pos;
            _vertexTextureCoordinate = textureCoordinate;
        }

        //Public methods for accessing the components of the custom vertex.
        public Vector3 Position
        {
            get { return _vertexPosition; }
            set { _vertexPosition = value; }
        }

        public Vector2 TextureCoordinate
        {
            get { return _vertexTextureCoordinate; }
            set { _vertexTextureCoordinate = value; }
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }
    }
}