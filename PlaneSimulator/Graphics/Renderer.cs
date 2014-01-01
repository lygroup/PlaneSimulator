﻿using System;
using System.Drawing;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Windows;
using PlaneSimulator.Graphics.Shaders;

namespace PlaneSimulator.Graphics
{
    class Renderer : IDisposable
    {
        public RenderForm Form { get; private set; }
        private readonly int _videoCardMemorySize;
        private readonly String _videoCardName;
        public int VideoCardMemorySize { get { return _videoCardMemorySize; } }
        public String VideoCardName { get { return _videoCardName; } }
        public Dx11 DirectX { get; private set; }
        public Camera Camera { get; set; }

        public Model Model { get; set; }

        public ColorShader ColorShader { get; set; }

        public Renderer()
        {
            CreateWindow();
            DirectX = new Dx11();
            DirectX.AcquireGpu(out _videoCardMemorySize, out _videoCardName);
            DirectX.CreateDeviceAndSwapChain(Form);
            DirectX.InitializeBuffers();
            DirectX.CreateMatrices();
            Camera = new Camera(new Vector3(0, 0, -10), Vector3.Zero);
            Model = new Model(DirectX.Device);
            ColorShader = new ColorShader(DirectX.Device);
        }

        private void CreateWindow()
        {
            Form = new RenderForm(ConfigurationManager.Config.Title)
            {
                ClientSize = new Size(ConfigurationManager.Config.Width, ConfigurationManager.Config.Height),
                FormBorderStyle = FormBorderStyle.FixedSingle
            };

            Form.Show();
        }

        public void Render()
        {
            DirectX.BeginScene(0.5f, 0.5f, 0.5f, 1f);
            
            // Put the model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            Model.Render(DirectX.Device.ImmediateContext);

            // Render the model using the color shader.
            ColorShader.Render(DirectX.Device.ImmediateContext, Model.IndexCount, DirectX.WorldMatrix, Camera.ViewMatrix, DirectX.ProjectionMatrix);

            DirectX.DrawScene();
        }

        public void Dispose()
        {
            DirectX.Dispose();
        }
    }
}
