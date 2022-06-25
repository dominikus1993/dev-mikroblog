
local_resource(
    'build',
    'dotnet publish -c Debug -o out',
    deps=['src/Api/DevMikroblog.Api'],
    ignore=['src/Api/DevMikroblog.Api/obj']
)

docker_build("devmikroblog", 'out', dockerfile="Dockerfile")

docker_compose("docker-compose.yml")