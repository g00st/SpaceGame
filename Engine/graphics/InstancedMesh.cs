using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace App.Engine;

public class InstancedMesh : Mesh
{
    private Dictionary<string,uint> _instanceAtributes = new Dictionary<string, uint>();
     private int instanceCount;
        public InstancedMesh(int instanceCount) : base()
        {
            this.instanceCount = instanceCount;
        }

        public void AddInstancedAtribute(Bufferlayout bufferlayout, float[] data, string name)
        {
            if (data.Length != instanceCount* bufferlayout.count)
            {
                throw new ArgumentException("InstanceatributesAtributes must aling with instancecount: " + data.Length + "   " + instanceCount);
            }
            var s = _vao.LinkAtribute(data, bufferlayout,1);
            Console.WriteLine("poss "+s);
            _instanceAtributes.Add(name,s);
        }
       
        public void ChangeInstancedAtributeData(Bufferlayout bufferlayout, float[] data, string name)
        {
            if (data.Length != instanceCount * bufferlayout.count && !_instanceAtributes.ContainsKey(name))
            {
                throw new ArgumentException("InstanceatributesAtributes must aling with instancecount: " + data.Length + "   " + instanceCount);
            }
            _vao.UpdateAttributeData(data, bufferlayout, _instanceAtributes[name]);

        }
        
     
        public override void Draw( DrawInfo drawInfo, Matrix4 view,Matrix4 Projection)
        {
         
         _vao.Bind();
         uint count = 0;
         foreach (var VARIABLE in _texture)
         {
            
             VARIABLE.Bind(count);
             count++;
         }
         Matrix4 Model = Matrix4.CreateScale(drawInfo.Size.X,drawInfo.Size.Y, 1.0f);
         Model *= Matrix4.CreateRotationZ(drawInfo.Rotation);
         Model *= Matrix4.CreateTranslation(drawInfo.Position.X,drawInfo.Position.Y,0);
        
        

         _shader.Bind();
         _shader.setUniformM4 ("u_Model",Model);
         _shader.setUniformM4 ("u_View",view);
         _shader.setUniformM4 ("u_Projection" ,Projection);
         GL.DrawElementsInstanced(PrimitiveType.Triangles, _Indecies.Length, DrawElementsType.UnsignedInt,IntPtr.Zero, instanceCount);
     }
}