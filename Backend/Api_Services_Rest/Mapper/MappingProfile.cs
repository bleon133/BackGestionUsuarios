using Api_Services_Rest.Domain.Models;
using Api_Services_Rest.Models.Dto;
using AutoMapper;
using System.Linq;

namespace Api_Services_Rest.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<direcciones, DireccionDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.Calle, opt => opt.MapFrom(src => src.calle))
                .ForMember(dest => dest.Colonia, opt => opt.MapFrom(src => src.colonia))
                .ForMember(dest => dest.NumInterior, opt => opt.MapFrom(src => src.NumInterior))
                .ForMember(dest => dest.NumExterior, opt => opt.MapFrom(src => src.NumExterior))
                .ForMember(dest => dest.Municipio, opt => opt.MapFrom(src => src.Municipio))
                .ForMember(dest => dest.IdPerfilUsuario, opt => opt.MapFrom(src => src.idPerfilUsuario))
                .ForMember(dest => dest.PerfilUsuarioDto, opt => opt.Ignore());

            CreateMap<DireccionDto, direcciones>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.calle, opt => opt.MapFrom(src => src.Calle))
                .ForMember(dest => dest.colonia, opt => opt.MapFrom(src => src.Colonia))
                .ForMember(dest => dest.NumInterior, opt => opt.MapFrom(src => src.NumInterior))
                .ForMember(dest => dest.NumExterior, opt => opt.MapFrom(src => src.NumExterior))
                .ForMember(dest => dest.Municipio, opt => opt.MapFrom(src => src.Municipio))
                .ForMember(dest => dest.idPerfilUsuario, opt => opt.MapFrom(src => src.IdPerfilUsuario))
                .ForMember(dest => dest.perfilUsuario, opt => opt.Ignore());

            // ── Telefonos ────────────────────────────────────────────────
            CreateMap<Telefonos, TelefonosDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.Celular, opt => opt.MapFrom(src => src.celular))
                .ForMember(dest => dest.Casa, opt => opt.MapFrom(src => src.casa))
                .ForMember(dest => dest.Oficina, opt => opt.MapFrom(src => src.oficina))
                .ForMember(dest => dest.IdPerfilUsuario, opt => opt.MapFrom(src => src.idPerfilUsuario))
                .ForMember(dest => dest.PerfilUsuarioDto, opt => opt.Ignore());

            CreateMap<TelefonosDto, Telefonos>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.celular, opt => opt.MapFrom(src => src.Celular))
                .ForMember(dest => dest.casa, opt => opt.MapFrom(src => src.Casa))
                .ForMember(dest => dest.oficina, opt => opt.MapFrom(src => src.Oficina))
                .ForMember(dest => dest.idPerfilUsuario, opt => opt.MapFrom(src => src.IdPerfilUsuario))
                .ForMember(dest => dest.perfilUsuario, opt => opt.Ignore());

            // ── perfilUsuario ────────────────────────────────────────────
            CreateMap<perfilUsuario, PerfilUsuarioDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.nombre))
                .ForMember(dest => dest.ApellidoPaterno, opt => opt.MapFrom(src => src.apellidoPaterno))
                .ForMember(dest => dest.ApellidoMaterno, opt => opt.MapFrom(src => src.apellidoMaterno))
                .ForMember(dest => dest.FechaNacimiento, opt => opt.MapFrom(src => src.fechaNacimiento))
                .ForMember(dest => dest.Rfc, opt => opt.MapFrom(src => src.rfc))
                .ForMember(dest => dest.IdUsuario, opt => opt.MapFrom(src => src.idUsuario))
                .ForMember(dest => dest.DireccionDtos, opt => opt.MapFrom(src => src.direcciones))
                .ForMember(dest => dest.TelefonosDtos, opt => opt.MapFrom(src => src.Telefonos));

            CreateMap<PerfilUsuarioDto, perfilUsuario>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.apellidoPaterno, opt => opt.MapFrom(src => src.ApellidoPaterno))
                .ForMember(dest => dest.apellidoMaterno, opt => opt.MapFrom(src => src.ApellidoMaterno))
                .ForMember(dest => dest.fechaNacimiento, opt => opt.MapFrom(src => src.FechaNacimiento))
                .ForMember(dest => dest.rfc, opt => opt.MapFrom(src => src.Rfc))
                .ForMember(dest => dest.idUsuario, opt => opt.MapFrom(src => src.IdUsuario))
                .ForMember(dest => dest.direcciones, opt => opt.Ignore())
                .ForMember(dest => dest.Telefonos, opt => opt.Ignore())
                .ForMember(dest => dest.usuarios, opt => opt.Ignore());

            // ── roles ────────────────────────────────────────────────────
            CreateMap<roles, RolesDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.Valor, opt => opt.MapFrom(src => src.valor))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.descripcion));

            CreateMap<RolesDto, roles>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.valor, opt => opt.MapFrom(src => src.Valor))
                .ForMember(dest => dest.descripcion, opt => opt.MapFrom(src => src.Descripcion))
                .ForMember(dest => dest.usuariosRoles, opt => opt.Ignore());

            // ── usuariosRoles ────────────────────────────────────────────
            CreateMap<usuariosRoles, UsuarioRolesDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.IdUsuario, opt => opt.MapFrom(src => src.idUsuario))
                .ForMember(dest => dest.IdRol, opt => opt.MapFrom(src => src.idRol))
                .ForMember(dest => dest.RolesDto, opt => opt.MapFrom(src => src.roles))
                .ForMember(dest => dest.UsuariosDto, opt => opt.Ignore());

            CreateMap<UsuarioRolesDto, usuariosRoles>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.idUsuario, opt => opt.MapFrom(src => src.IdUsuario))
                .ForMember(dest => dest.idRol, opt => opt.MapFrom(src => src.IdRol))
                .ForMember(dest => dest.roles, opt => opt.Ignore())
                .ForMember(dest => dest.usuarios, opt => opt.Ignore());

            // ── usuarios ─────────────────────────────────────────────────
            CreateMap<usuarios, UsuariosDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.username))
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.Suspendido, opt => opt.MapFrom(src => src.suspendido))
                .ForMember(dest => dest.UsuariosRolesDtos, opt => opt.MapFrom(src => src.usuariosRoles))
                .ForMember(dest => dest.PerfilUsuarioDtos, opt => opt.MapFrom(src => src.perfilUsuario));

            CreateMap<UsuariosDto, usuarios>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.password, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.suspendido, opt => opt.MapFrom(src => src.Suspendido))
                .ForMember(dest => dest.usuariosRoles, opt => opt.Ignore())
                .ForMember(dest => dest.perfilUsuario, opt => opt.Ignore());
        }
    }
}