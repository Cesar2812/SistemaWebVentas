using Firebase.Auth;
using Firebase.Storage;
using LayerBusiness.Interface;
using LayerDataBase.Interface;
using LayerEntity;




namespace LayerBusiness.Implementation;


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

            //token de cancelacion
            var cancellation= new CancellationTokenSource();

            var task = new FirebaseStorage(

                config["ruta"],
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true //si ocurre error cancela
                }
            ).Child(config[destinationFolder]).Child(config[fileName]).PutAsync(streamFile, cancellation.Token);

            urlImage = await task;
        }
        catch
        {
            urlImage = "";
        }
        return urlImage;
    }

    public async Task<bool> DeleteStorage(string destinationFolder, string fileName)
    {
        IQueryable<Configuracion> query = await _repository.Consult(c => c.Recurso.Equals("FireBase_Storage"));
        Dictionary<string, string> config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

        var auth = new FirebaseAuthProvider(new FirebaseConfig(config["api_key"]));
        var a = await auth.SignInWithEmailAndPasswordAsync(config["email"], config["clave"]);

        //token de cancelacion
        var cancellation = new CancellationTokenSource();

        var task = new FirebaseStorage(

            config["ruta"],
            new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                ThrowOnCancel = true //si ocurre error cancela
            }
        ).Child(config[destinationFolder]).Child(config[fileName]).DeleteAsync();

        await task;
        return true;
    }
}
