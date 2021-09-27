define(["./Cartesian2-b4b7b0b3","./Check-5e798bbf","./when-208fe5b0","./EllipseGeometry-8d2f5a72","./VertexFormat-7e57a3bd","./Math-8386669c","./GeometryOffsetAttribute-3497d4dd","./Transforms-d5dbea8d","./RuntimeError-7f634f5d","./ComponentDatatype-ce69354e","./WebGLConstants-76bb35d1","./EllipseGeometryLibrary-f934d063","./GeometryAttribute-3314089a","./GeometryAttributes-b0b294d8","./GeometryInstance-3ec54139","./GeometryPipeline-cd170892","./AttributeCompression-9711314b","./EncodedCartesian3-21af0f3b","./IndexDatatype-da7c58eb","./IntersectionTests-eb44dd99","./Plane-06d5a63d"],function(r,e,o,n,s,t,i,a,l,d,m,u,p,c,y,_,h,G,x,f,g){"use strict";function b(e){var t=(e=o.defaultValue(e,o.defaultValue.EMPTY_OBJECT)).radius,e={center:e.center,semiMajorAxis:t,semiMinorAxis:t,ellipsoid:e.ellipsoid,height:e.height,extrudedHeight:e.extrudedHeight,granularity:e.granularity,vertexFormat:e.vertexFormat,stRotation:e.stRotation,shadowVolume:e.shadowVolume};this._ellipseGeometry=new n.EllipseGeometry(e),this._workerName="createCircleGeometry"}b.packedLength=n.EllipseGeometry.packedLength,b.pack=function(e,t,i){return n.EllipseGeometry.pack(e._ellipseGeometry,t,i)};var E=new n.EllipseGeometry({center:new r.Cartesian3,semiMajorAxis:1,semiMinorAxis:1}),v={center:new r.Cartesian3,radius:void 0,ellipsoid:r.Ellipsoid.clone(r.Ellipsoid.UNIT_SPHERE),height:void 0,extrudedHeight:void 0,granularity:void 0,vertexFormat:new s.VertexFormat,stRotation:void 0,semiMajorAxis:void 0,semiMinorAxis:void 0,shadowVolume:void 0};return b.unpack=function(e,t,i){t=n.EllipseGeometry.unpack(e,t,E);return v.center=r.Cartesian3.clone(t._center,v.center),v.ellipsoid=r.Ellipsoid.clone(t._ellipsoid,v.ellipsoid),v.height=t._height,v.extrudedHeight=t._extrudedHeight,v.granularity=t._granularity,v.vertexFormat=s.VertexFormat.clone(t._vertexFormat,v.vertexFormat),v.stRotation=t._stRotation,v.shadowVolume=t._shadowVolume,o.defined(i)?(v.semiMajorAxis=t._semiMajorAxis,v.semiMinorAxis=t._semiMinorAxis,i._ellipseGeometry=new n.EllipseGeometry(v),i):(v.radius=t._semiMajorAxis,new b(v))},b.createGeometry=function(e){return n.EllipseGeometry.createGeometry(e._ellipseGeometry)},b.createShadowVolume=function(e,t,i){var r=e._ellipseGeometry._granularity,o=e._ellipseGeometry._ellipsoid,t=t(r,o),i=i(r,o);return new b({center:e._ellipseGeometry._center,radius:e._ellipseGeometry._semiMajorAxis,ellipsoid:o,stRotation:e._ellipseGeometry._stRotation,granularity:r,extrudedHeight:t,height:i,vertexFormat:s.VertexFormat.POSITION_ONLY,shadowVolume:!0})},Object.defineProperties(b.prototype,{rectangle:{get:function(){return this._ellipseGeometry.rectangle}},textureCoordinateRotationPoints:{get:function(){return this._ellipseGeometry.textureCoordinateRotationPoints}}}),function(e,t){return(e=o.defined(t)?b.unpack(e,t):e)._ellipseGeometry._center=r.Cartesian3.clone(e._ellipseGeometry._center),e._ellipseGeometry._ellipsoid=r.Ellipsoid.clone(e._ellipseGeometry._ellipsoid),b.createGeometry(e)}});