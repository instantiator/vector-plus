using System.Collections.Generic;

namespace VectorPlus.Lib.Vision
{
    public class CameraFrameProcessingResult
    {
        /// <summary>
        /// The original image as a byte array.
        /// </summary>
        public byte[] Image { get; set; }

        /// <summary>
        /// A set of bounding boxes showing objects found in the image.
        /// NB. Because the dimensions of bounding boxes correspond to
        /// the model input of 416 x 416, remember to scale the bounding
        /// box for overlaying on images if doing that.
        /// </summary>
        public IEnumerable<IdentifiedObjectBoundingBox> Boxes { get; set; }
    }
}
