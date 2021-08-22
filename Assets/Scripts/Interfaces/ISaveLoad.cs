
namespace Interfaces {
    public interface ISaveLoad {

        EntitySaveData Save();

        void Load(EntitySaveData loadedData);

    }
}
