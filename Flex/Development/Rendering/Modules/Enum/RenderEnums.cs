using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Development.Rendering.Modules.Enum
{
    public enum SelectionType
    {
        HOVER,
        DRAG,
        SELECT,
        DELETE
    }

    public enum TransformDragging
    {
        X,
        Y,
        Z,
        NONE
    }

    public enum DirectionalTransformDragging
    {
        XA,
        XB,
        YA,
        YB,
        ZA,
        ZB,
        NONE
    }
}
