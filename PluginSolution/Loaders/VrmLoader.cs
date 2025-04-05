#if PLUGIN
using System.Collections;

namespace ValheimPlayerModels.Loaders
{
    public class VrmLoader : AvatarLoaderBase
    {
        public override IEnumerator LoadFile(string file)
        {
            LoadedSuccessfully = false;
            yield break;
        }

        public override IEnumerator LoadAvatar(PlayerModel playerModel)
        {
            yield break;
        }

        public override IEnumerator Unload()
        {
            yield break;
        }
    }
}
#endif