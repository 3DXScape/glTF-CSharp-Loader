import * as Extras from './Extras';
import * as FrameTransform from './FrameTransform';
import * as Orientation from './Orientation';
import * as GeoPose from './GeoPose';

/// <summary>
/// Advanced GeoPose.
/// </summary>
export class Advanced extends GeoPose.GeoPose {
    public constructor(poseID: Extras.PoseID, frameTransform: FrameTransform.Extrinsic, orientation: Orientation.Quaternion) {
        super();
        this.poseID = poseID;
        this.FrameTransform = frameTransform;
        this.Orientation = orientation;
    }

    /// <summary>
    /// A Frame Specification defining a frame with associated coordinate system whose Position is the origin.
    /// </summary>
    public override FrameTransform: FrameTransform.Extrinsic;

    /// <summary>
    /// An Orientation specified as a unit quaternion.
    /// </summary>
    public override Orientation: Orientation.Quaternion;
}
