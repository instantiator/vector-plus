using System;
namespace VectorPlus.Capabilities.Vision.Yolo
{
    public class YoloNamer
    {
        public static string ParseLabel(string labelStr)
        {
            YoloLabels label;
            bool found = Enum.TryParse(labelStr, out label);

            if (!found)
            {
                return null;
            }
            else
            {
                switch (label)
                {
                    case YoloLabels.aeroplane:
                        return "an aeroplane";
                    case YoloLabels.bicycle:
                        return "a bicycle";
                    case YoloLabels.bird:
                        return "a bird";
                    case YoloLabels.boat:
                        return "a boat";
                    case YoloLabels.bottle:
                        return "a bottle";
                    case YoloLabels.bus:
                        return "a bus";
                    case YoloLabels.car:
                        return "a car";
                    case YoloLabels.cat:
                        return "kitty!";
                    case YoloLabels.chair:
                        return "a chair";
                    case YoloLabels.cow:
                        return "a cow";
                    case YoloLabels.diningtable:
                        return "a table";
                    case YoloLabels.dog:
                        return "a dog";
                    case YoloLabels.horse:
                        return "a dog";
                    case YoloLabels.motorbike:
                        return "a cool motorbike";
                    case YoloLabels.person:
                        return "someone";
                    case YoloLabels.pottedplant:
                        return "a plant";
                    case YoloLabels.sheep:
                        return "a sheep";
                    case YoloLabels.sofa:
                        return "a sofa";
                    case YoloLabels.train:
                        return "a train";
                    case YoloLabels.tvmonitor:
                        return "a screen";

                    default:
                        return null;
                }
            }


            }

    }
}
