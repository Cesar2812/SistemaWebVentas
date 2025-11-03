using Firebase.Auth;
using LayerBusiness.Interface;
using LayerDataBase.Interface;
using LayerEntity;

namespace LayerBusiness.Implementation
{
    public class FireBaseService:IFireBaseService
    {

        private readonly IGenericRepository<Configuracion> _repository;

        public FireBaseService(IGenericRepository<Configuracion> repository)
        {
            _repository = repository;
        }

        public async Task<string> LoadStorage(Stream streamFile, string destinationFolder, string fileName)
        {

            string urlImage = "";

            try
            {

                IQueryable<Configuracion> query = await _repository.Consult(c => c.Recurso.Equals("FireBase_Storage"));
                Dictionary<string, string> config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var auth = new FirebaseAuthProvider(new FirebaseConfig(config["api_key"]));
                var a = await auth.SignInWithEmailAndPasswordAsync(config["email"], config["clave"]);

            }
            catch
            {

            }

        }

        public Task<string> DeleteStorage(string destinationFolder, string fileName)
        {
            throw new NotImplementedException();
        }


    }
}
