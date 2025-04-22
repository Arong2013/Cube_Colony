#include "pch-cpp.hpp"





template <typename R, typename T1, typename T2>
struct VirtualFuncInvoker2
{
	typedef R (*Func)(void*, T1, T2, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1 p1, T2 p2)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		return ((Func)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
	}
};
template <typename R, typename T1, typename T2>
struct VirtualFuncInvoker2Invoker;
template <typename R, typename T1, typename T2>
struct VirtualFuncInvoker2Invoker<R, T1*, T2*>
{
	static inline R Invoke (Il2CppMethodSlot slot, RuntimeObject* obj, T1* p1, T2* p2)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		R ret;
		void* params[2] = { p1, p2 };
		invokeData.method->invoker_method(il2cpp_codegen_get_method_pointer(invokeData.method), invokeData.method, obj, params, &ret);
		return ret;
	}
};

struct Dictionary_2_t14FE4A752A83D53771C584E4C8D14E01F2AFD7BA;
struct Dictionary_2_t9D50BDE1AE9A04E8610682F97C4DB702D3A9B4D3;
struct IEqualityComparer_1_tAE94C8F24AD5B94D4EE85CA9FC59E3409D41CAF7;
struct KeyCollection_t018A8DACA62B00F41314B17E9BB03E1169319CA5;
struct List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD;
struct ValueCollection_t26762FBAABEED8AF6FEC2532717F621A053E1E81;
struct fsDirectConverter_1_tCBA7B5D48312C1775A21082D3D453731C902A8BD;
struct EntryU5BU5D_tCFEDD9B392BFDE2D313BC0974DB2CABF3A7A893D;
struct CharU5BU5D_t799905CF001DD5F13F7DBB310181FC4D8B7D0AAB;
struct Int32U5BU5D_t19C97395396A72ECAF310612F0760F165060314C;
struct IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832;
struct StackTraceU5BU5D_t32FBCB20930EAF5BAE3F450FF75228E5450DA0DF;
struct StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248;
struct TypeU5BU5D_t97234E1129B564EB38B8D85CAC2AD8B5B9522FFB;
struct fsVersionedTypeU5BU5D_tF4A5B18C49769943892747B1A1040CAE86746E1F;
struct Binder_t91BFCE95A7057FADF4D8A1A342AFE52872246235;
struct IDictionary_t6D03155AF1FA9083817AA5B6AD7DEEACC26AB220;
struct InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB;
struct MemberFilter_tF644F1AE82F611B677CE1964D5A3277DDA21D553;
struct SafeSerializationManager_tCBB85B95DFD1634237140CD892E82D06ECB3F5E6;
struct String_t;
struct Type_t;
struct Void_t4861ACF8F4594C3437BB48B6E56783494B843915;
struct fsBaseConverter_tDF9366828A5B8ADB6D986EB35B320D4DDC81CB04;
struct fsData_t6E144EC58C6E449805AB7BF77BCDAB7230BBFA23;
struct fsDirectConverter_t9EE506A371C95C62E21CA6586396D7B3BD55F668;
struct fsSerializer_t770DD30E2427CF63ECAEF61D7A1D1D178CECB3DC;

IL2CPP_EXTERN_C RuntimeClass* Dictionary_2_t9D50BDE1AE9A04E8610682F97C4DB702D3A9B4D3_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* fsData_t6E144EC58C6E449805AB7BF77BCDAB7230BBFA23_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C String_t* _stringLiteral0A7D3B18DF4F9238A50A156FF06A5A7E794C1C7F;
IL2CPP_EXTERN_C const RuntimeMethod* Dictionary_2__ctor_m3DC0B06262330D5590CF0BE04759F02C53807E04_RuntimeMethod_var;
struct Exception_t_marshaled_com;
struct Exception_t_marshaled_pinvoke;
struct fsVersionedType_tE569BD045F7DEDEC2122CC6CC0023B62B95332E8_marshaled_com;
struct fsVersionedType_tE569BD045F7DEDEC2122CC6CC0023B62B95332E8_marshaled_pinvoke;


IL2CPP_EXTERN_C_BEGIN
IL2CPP_EXTERN_C_END

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
struct Dictionary_2_t9D50BDE1AE9A04E8610682F97C4DB702D3A9B4D3  : public RuntimeObject
{
	Int32U5BU5D_t19C97395396A72ECAF310612F0760F165060314C* ____buckets;
	EntryU5BU5D_tCFEDD9B392BFDE2D313BC0974DB2CABF3A7A893D* ____entries;
	int32_t ____count;
	int32_t ____freeList;
	int32_t ____freeCount;
	int32_t ____version;
	RuntimeObject* ____comparer;
	KeyCollection_t018A8DACA62B00F41314B17E9BB03E1169319CA5* ____keys;
	ValueCollection_t26762FBAABEED8AF6FEC2532717F621A053E1E81* ____values;
	RuntimeObject* ____syncRoot;
};
struct MemberInfo_t  : public RuntimeObject
{
};
struct String_t  : public RuntimeObject
{
	int32_t ____stringLength;
	Il2CppChar ____firstChar;
};
struct ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F  : public RuntimeObject
{
};
struct ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F_marshaled_pinvoke
{
};
struct ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F_marshaled_com
{
};
struct fsBaseConverter_tDF9366828A5B8ADB6D986EB35B320D4DDC81CB04  : public RuntimeObject
{
	fsSerializer_t770DD30E2427CF63ECAEF61D7A1D1D178CECB3DC* ___Serializer;
};
struct fsData_t6E144EC58C6E449805AB7BF77BCDAB7230BBFA23  : public RuntimeObject
{
	RuntimeObject* ____value;
};
typedef Il2CppFullySharedGenericStruct fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88;
struct Boolean_t09A6377A54BE2F9E6985A8149F19234FD7DDFE22 
{
	bool ___m_value;
};
struct Enum_t2A1A94B24E3B776EEF4E5E485E290BB9D4D072E2  : public ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F
{
};
struct Enum_t2A1A94B24E3B776EEF4E5E485E290BB9D4D072E2_marshaled_pinvoke
{
};
struct Enum_t2A1A94B24E3B776EEF4E5E485E290BB9D4D072E2_marshaled_com
{
};
struct IntPtr_t 
{
	void* ___m_value;
};
struct Void_t4861ACF8F4594C3437BB48B6E56783494B843915 
{
	union
	{
		struct
		{
		};
		uint8_t Void_t4861ACF8F4594C3437BB48B6E56783494B843915__padding[1];
	};
};
struct fsDirectConverter_t9EE506A371C95C62E21CA6586396D7B3BD55F668  : public fsBaseConverter_tDF9366828A5B8ADB6D986EB35B320D4DDC81CB04
{
};
struct fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974 
{
	bool ____success;
	List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* ____messages;
};
struct fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974_marshaled_pinvoke
{
	int32_t ____success;
	List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* ____messages;
};
struct fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974_marshaled_com
{
	int32_t ____success;
	List_1_tF470A3BE5C1B5B68E1325EF3F109D172E60BD7CD* ____messages;
};
struct fsVersionedType_tE569BD045F7DEDEC2122CC6CC0023B62B95332E8 
{
	fsVersionedTypeU5BU5D_tF4A5B18C49769943892747B1A1040CAE86746E1F* ___Ancestors;
	String_t* ___VersionString;
	Type_t* ___ModelType;
};
struct fsVersionedType_tE569BD045F7DEDEC2122CC6CC0023B62B95332E8_marshaled_pinvoke
{
	fsVersionedType_tE569BD045F7DEDEC2122CC6CC0023B62B95332E8_marshaled_pinvoke* ___Ancestors;
	char* ___VersionString;
	Type_t* ___ModelType;
};
struct fsVersionedType_tE569BD045F7DEDEC2122CC6CC0023B62B95332E8_marshaled_com
{
	fsVersionedType_tE569BD045F7DEDEC2122CC6CC0023B62B95332E8_marshaled_com* ___Ancestors;
	Il2CppChar* ___VersionString;
	Type_t* ___ModelType;
};
struct fsDirectConverter_1_tCBA7B5D48312C1775A21082D3D453731C902A8BD  : public fsDirectConverter_t9EE506A371C95C62E21CA6586396D7B3BD55F668
{
};
struct fsOption_1_t5307CE32C845EE97F1173CDBB2385C4B81D2A28A 
{
	bool ____hasValue;
	fsVersionedType_tE569BD045F7DEDEC2122CC6CC0023B62B95332E8 ____value;
};
struct Exception_t  : public RuntimeObject
{
	String_t* ____className;
	String_t* ____message;
	RuntimeObject* ____data;
	Exception_t* ____innerException;
	String_t* ____helpURL;
	RuntimeObject* ____stackTrace;
	String_t* ____stackTraceString;
	String_t* ____remoteStackTraceString;
	int32_t ____remoteStackIndex;
	RuntimeObject* ____dynamicMethods;
	int32_t ____HResult;
	String_t* ____source;
	SafeSerializationManager_tCBB85B95DFD1634237140CD892E82D06ECB3F5E6* ____safeSerializationManager;
	StackTraceU5BU5D_t32FBCB20930EAF5BAE3F450FF75228E5450DA0DF* ___captured_traces;
	IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___native_trace_ips;
	int32_t ___caught_in_unmanaged;
};
struct Exception_t_marshaled_pinvoke
{
	char* ____className;
	char* ____message;
	RuntimeObject* ____data;
	Exception_t_marshaled_pinvoke* ____innerException;
	char* ____helpURL;
	Il2CppIUnknown* ____stackTrace;
	char* ____stackTraceString;
	char* ____remoteStackTraceString;
	int32_t ____remoteStackIndex;
	Il2CppIUnknown* ____dynamicMethods;
	int32_t ____HResult;
	char* ____source;
	SafeSerializationManager_tCBB85B95DFD1634237140CD892E82D06ECB3F5E6* ____safeSerializationManager;
	StackTraceU5BU5D_t32FBCB20930EAF5BAE3F450FF75228E5450DA0DF* ___captured_traces;
	Il2CppSafeArray* ___native_trace_ips;
	int32_t ___caught_in_unmanaged;
};
struct Exception_t_marshaled_com
{
	Il2CppChar* ____className;
	Il2CppChar* ____message;
	RuntimeObject* ____data;
	Exception_t_marshaled_com* ____innerException;
	Il2CppChar* ____helpURL;
	Il2CppIUnknown* ____stackTrace;
	Il2CppChar* ____stackTraceString;
	Il2CppChar* ____remoteStackTraceString;
	int32_t ____remoteStackIndex;
	Il2CppIUnknown* ____dynamicMethods;
	int32_t ____HResult;
	Il2CppChar* ____source;
	SafeSerializationManager_tCBB85B95DFD1634237140CD892E82D06ECB3F5E6* ____safeSerializationManager;
	StackTraceU5BU5D_t32FBCB20930EAF5BAE3F450FF75228E5450DA0DF* ___captured_traces;
	Il2CppSafeArray* ___native_trace_ips;
	int32_t ___caught_in_unmanaged;
};
struct RuntimeTypeHandle_t332A452B8B6179E4469B69525D0FE82A88030F7B 
{
	intptr_t ___value;
};
struct fsDataType_t4DDBD7DB585B530CF852D23D5FF4D9801DDE4A16 
{
	int32_t ___value__;
};
struct SystemException_tCC48D868298F4C0705279823E34B00F4FBDB7295  : public Exception_t
{
};
struct Type_t  : public MemberInfo_t
{
	RuntimeTypeHandle_t332A452B8B6179E4469B69525D0FE82A88030F7B ____impl;
};
struct InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB  : public SystemException_tCC48D868298F4C0705279823E34B00F4FBDB7295
{
};
struct String_t_StaticFields
{
	String_t* ___Empty;
};
struct fsData_t6E144EC58C6E449805AB7BF77BCDAB7230BBFA23_StaticFields
{
	fsData_t6E144EC58C6E449805AB7BF77BCDAB7230BBFA23* ___True;
	fsData_t6E144EC58C6E449805AB7BF77BCDAB7230BBFA23* ___False;
	fsData_t6E144EC58C6E449805AB7BF77BCDAB7230BBFA23* ___Null;
};
struct Boolean_t09A6377A54BE2F9E6985A8149F19234FD7DDFE22_StaticFields
{
	String_t* ___TrueString;
	String_t* ___FalseString;
};
struct fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974_StaticFields
{
	StringU5BU5D_t7674CD946EC0CE7B3AE0BE70E6EE85F2ECD9F248* ___EmptyStringArray;
	fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974 ___Success;
};
struct fsOption_1_t5307CE32C845EE97F1173CDBB2385C4B81D2A28A_StaticFields
{
	fsOption_1_t5307CE32C845EE97F1173CDBB2385C4B81D2A28A ___Empty;
};
struct Type_t_StaticFields
{
	Binder_t91BFCE95A7057FADF4D8A1A342AFE52872246235* ___s_defaultBinder;
	Il2CppChar ___Delimiter;
	TypeU5BU5D_t97234E1129B564EB38B8D85CAC2AD8B5B9522FFB* ___EmptyTypes;
	RuntimeObject* ___Missing;
	MemberFilter_tF644F1AE82F611B677CE1964D5A3277DDA21D553* ___FilterAttribute;
	MemberFilter_tF644F1AE82F611B677CE1964D5A3277DDA21D553* ___FilterName;
	MemberFilter_tF644F1AE82F611B677CE1964D5A3277DDA21D553* ___FilterNameIgnoreCase;
};
#ifdef __clang__
#pragma clang diagnostic pop
#endif


IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Dictionary_2__ctor_m5B32FBC624618211EB461D59CFBB10E987FD1329_gshared (Dictionary_2_t14FE4A752A83D53771C584E4C8D14E01F2AFD7BA* __this, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR bool fsOption_1_get_HasValue_m2E9021BB4CB6156A14918A5B865D21751CA6D00C_gshared_inline (fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool fsOption_1_get_IsEmpty_m8F02E0608720E562CDB4C8906C1CD863FD8FE767_gshared (fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void fsOption_1_get_Value_m2A0C3FDC3098EA4DC3BB5CF398E95EC2C991988A_gshared (fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88* __this, Il2CppFullySharedGenericAny* il2cppRetVal, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void fsOption_1__ctor_m343A7CF35E302DB9E5BCA27A0DAC03751B238516_gshared (fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88* __this, Il2CppFullySharedGenericAny ___0_value, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR bool fsOption_1_get_HasValue_mA83E56B4674C678E7C760EABB73397EF37356E6F_gshared_inline (fsOption_1_t5307CE32C845EE97F1173CDBB2385C4B81D2A28A* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool fsOption_1_get_IsEmpty_m95C6343EC3B5E180C487DF2F28019391A5B6672E_gshared (fsOption_1_t5307CE32C845EE97F1173CDBB2385C4B81D2A28A* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR fsVersionedType_tE569BD045F7DEDEC2122CC6CC0023B62B95332E8 fsOption_1_get_Value_m06DFFCCEB8E41DDEAEC89C3133DC3E75E3EA2241_gshared (fsOption_1_t5307CE32C845EE97F1173CDBB2385C4B81D2A28A* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void fsOption_1__ctor_m22E0CD4999637C5B1CCFAD980FD29F2FFE3963B6_gshared (fsOption_1_t5307CE32C845EE97F1173CDBB2385C4B81D2A28A* __this, fsVersionedType_tE569BD045F7DEDEC2122CC6CC0023B62B95332E8 ___0_value, const RuntimeMethod* method) ;

IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Type_t* Type_GetTypeFromHandle_m6062B81682F79A4D6DF2640692EE6D9987858C57 (RuntimeTypeHandle_t332A452B8B6179E4469B69525D0FE82A88030F7B ___0_handle, const RuntimeMethod* method) ;
inline void Dictionary_2__ctor_m3DC0B06262330D5590CF0BE04759F02C53807E04 (Dictionary_2_t9D50BDE1AE9A04E8610682F97C4DB702D3A9B4D3* __this, const RuntimeMethod* method)
{
	((  void (*) (Dictionary_2_t9D50BDE1AE9A04E8610682F97C4DB702D3A9B4D3*, const RuntimeMethod*))Dictionary_2__ctor_m5B32FBC624618211EB461D59CFBB10E987FD1329_gshared)(__this, method);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void fsData__ctor_mCF4D73BFD9271596000ACC3E17988E3492236781 (fsData_t6E144EC58C6E449805AB7BF77BCDAB7230BBFA23* __this, Dictionary_2_t9D50BDE1AE9A04E8610682F97C4DB702D3A9B4D3* ___0_dict, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974 fsBaseConverter_CheckType_m14C4D0CAF424DF9F14D6365B5E5681C43FA82F32 (fsBaseConverter_tDF9366828A5B8ADB6D986EB35B320D4DDC81CB04* __this, fsData_t6E144EC58C6E449805AB7BF77BCDAB7230BBFA23* ___0_data, int32_t ___1_type, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974 fsResult_op_Addition_mA94A4AD68668E539DEFE1255DC72B9D11A6DE41C (fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974 ___0_a, fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974 ___1_b, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool fsResult_get_Failed_m1398C627A72E75F5C7F8DB1A7C14E5B3271FF1FB (fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Dictionary_2_t9D50BDE1AE9A04E8610682F97C4DB702D3A9B4D3* fsData_get_AsDictionary_m51779E71BBC994A7F5036BFB43F61B28C0817D86 (fsData_t6E144EC58C6E449805AB7BF77BCDAB7230BBFA23* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void fsDirectConverter__ctor_mCE6F7898DA2A3FBE101F5939D06A809E3164ABE1 (fsDirectConverter_t9EE506A371C95C62E21CA6586396D7B3BD55F668* __this, const RuntimeMethod* method) ;
inline bool fsOption_1_get_HasValue_m2E9021BB4CB6156A14918A5B865D21751CA6D00C_inline (fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88* __this, const RuntimeMethod* method)
{
	return ((  bool (*) (fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88*, const RuntimeMethod*))fsOption_1_get_HasValue_m2E9021BB4CB6156A14918A5B865D21751CA6D00C_gshared_inline)(__this, method);
}
inline bool fsOption_1_get_IsEmpty_m8F02E0608720E562CDB4C8906C1CD863FD8FE767 (fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88* __this, const RuntimeMethod* method)
{
	return ((  bool (*) (fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88*, const RuntimeMethod*))fsOption_1_get_IsEmpty_m8F02E0608720E562CDB4C8906C1CD863FD8FE767_gshared)(__this, method);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162 (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* __this, String_t* ___0_message, const RuntimeMethod* method) ;
inline void fsOption_1_get_Value_m2A0C3FDC3098EA4DC3BB5CF398E95EC2C991988A (fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88* __this, Il2CppFullySharedGenericAny* il2cppRetVal, const RuntimeMethod* method)
{
	((  void (*) (fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88*, Il2CppFullySharedGenericAny*, const RuntimeMethod*))fsOption_1_get_Value_m2A0C3FDC3098EA4DC3BB5CF398E95EC2C991988A_gshared)((fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88*)__this, il2cppRetVal, method);
}
inline void fsOption_1__ctor_m343A7CF35E302DB9E5BCA27A0DAC03751B238516 (fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88* __this, Il2CppFullySharedGenericAny ___0_value, const RuntimeMethod* method)
{
	((  void (*) (fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88*, Il2CppFullySharedGenericAny, const RuntimeMethod*))fsOption_1__ctor_m343A7CF35E302DB9E5BCA27A0DAC03751B238516_gshared)((fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88*)__this, ___0_value, method);
}
inline bool fsOption_1_get_HasValue_mA83E56B4674C678E7C760EABB73397EF37356E6F_inline (fsOption_1_t5307CE32C845EE97F1173CDBB2385C4B81D2A28A* __this, const RuntimeMethod* method)
{
	return ((  bool (*) (fsOption_1_t5307CE32C845EE97F1173CDBB2385C4B81D2A28A*, const RuntimeMethod*))fsOption_1_get_HasValue_mA83E56B4674C678E7C760EABB73397EF37356E6F_gshared_inline)(__this, method);
}
inline bool fsOption_1_get_IsEmpty_m95C6343EC3B5E180C487DF2F28019391A5B6672E (fsOption_1_t5307CE32C845EE97F1173CDBB2385C4B81D2A28A* __this, const RuntimeMethod* method)
{
	return ((  bool (*) (fsOption_1_t5307CE32C845EE97F1173CDBB2385C4B81D2A28A*, const RuntimeMethod*))fsOption_1_get_IsEmpty_m95C6343EC3B5E180C487DF2F28019391A5B6672E_gshared)(__this, method);
}
inline fsVersionedType_tE569BD045F7DEDEC2122CC6CC0023B62B95332E8 fsOption_1_get_Value_m06DFFCCEB8E41DDEAEC89C3133DC3E75E3EA2241 (fsOption_1_t5307CE32C845EE97F1173CDBB2385C4B81D2A28A* __this, const RuntimeMethod* method)
{
	return ((  fsVersionedType_tE569BD045F7DEDEC2122CC6CC0023B62B95332E8 (*) (fsOption_1_t5307CE32C845EE97F1173CDBB2385C4B81D2A28A*, const RuntimeMethod*))fsOption_1_get_Value_m06DFFCCEB8E41DDEAEC89C3133DC3E75E3EA2241_gshared)(__this, method);
}
inline void fsOption_1__ctor_m22E0CD4999637C5B1CCFAD980FD29F2FFE3963B6 (fsOption_1_t5307CE32C845EE97F1173CDBB2385C4B81D2A28A* __this, fsVersionedType_tE569BD045F7DEDEC2122CC6CC0023B62B95332E8 ___0_value, const RuntimeMethod* method)
{
	((  void (*) (fsOption_1_t5307CE32C845EE97F1173CDBB2385C4B81D2A28A*, fsVersionedType_tE569BD045F7DEDEC2122CC6CC0023B62B95332E8, const RuntimeMethod*))fsOption_1__ctor_m22E0CD4999637C5B1CCFAD980FD29F2FFE3963B6_gshared)(__this, ___0_value, method);
}
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Type_t* fsDirectConverter_1_get_ModelType_m7BFCBE6128959F6AAA08BB9FB82C5E3DA07FB3AD_gshared (fsDirectConverter_1_tCBA7B5D48312C1775A21082D3D453731C902A8BD* __this, const RuntimeMethod* method) 
{
	{
		RuntimeTypeHandle_t332A452B8B6179E4469B69525D0FE82A88030F7B L_0 = { reinterpret_cast<intptr_t> (il2cpp_rgctx_type(method->klass->rgctx_data, 0)) };
		il2cpp_codegen_runtime_class_init_inline(il2cpp_defaults.systemtype_class);
		Type_t* L_1;
		L_1 = Type_GetTypeFromHandle_m6062B81682F79A4D6DF2640692EE6D9987858C57(L_0, NULL);
		return L_1;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974 fsDirectConverter_1_TrySerialize_mAB417033757D14765392C802374EE82941F7E430_gshared (fsDirectConverter_1_tCBA7B5D48312C1775A21082D3D453731C902A8BD* __this, RuntimeObject* ___0_instance, fsData_t6E144EC58C6E449805AB7BF77BCDAB7230BBFA23** ___1_serialized, Type_t* ___2_storageType, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Dictionary_2__ctor_m3DC0B06262330D5590CF0BE04759F02C53807E04_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Dictionary_2_t9D50BDE1AE9A04E8610682F97C4DB702D3A9B4D3_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&fsData_t6E144EC58C6E449805AB7BF77BCDAB7230BBFA23_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	const uint32_t SizeOf_TModel_tA846BAB14AD72C3F97B08FAFEF7B2629C8054C13 = il2cpp_codegen_sizeof(il2cpp_rgctx_data_no_init(method->klass->rgctx_data, 1));
	const Il2CppFullySharedGenericAny L_2 = alloca(SizeOf_TModel_tA846BAB14AD72C3F97B08FAFEF7B2629C8054C13);
	Dictionary_2_t9D50BDE1AE9A04E8610682F97C4DB702D3A9B4D3* V_0 = NULL;
	{
		Dictionary_2_t9D50BDE1AE9A04E8610682F97C4DB702D3A9B4D3* L_0 = (Dictionary_2_t9D50BDE1AE9A04E8610682F97C4DB702D3A9B4D3*)il2cpp_codegen_object_new(Dictionary_2_t9D50BDE1AE9A04E8610682F97C4DB702D3A9B4D3_il2cpp_TypeInfo_var);
		Dictionary_2__ctor_m3DC0B06262330D5590CF0BE04759F02C53807E04(L_0, Dictionary_2__ctor_m3DC0B06262330D5590CF0BE04759F02C53807E04_RuntimeMethod_var);
		V_0 = L_0;
		RuntimeObject* L_1 = ___0_instance;
		void* L_3 = UnBox_Any(L_1, il2cpp_rgctx_data(method->klass->rgctx_data, 1), L_2);
		Dictionary_2_t9D50BDE1AE9A04E8610682F97C4DB702D3A9B4D3* L_4 = V_0;
		fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974 L_5;
		L_5 = VirtualFuncInvoker2Invoker< fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974, Il2CppFullySharedGenericAny, Dictionary_2_t9D50BDE1AE9A04E8610682F97C4DB702D3A9B4D3* >::Invoke(10, __this, (il2cpp_codegen_class_is_value_type(il2cpp_rgctx_data_no_init(method->klass->rgctx_data, 1)) ? (((Il2CppFullySharedGenericAny)(Il2CppFullySharedGenericAny*)L_3)): *(void**)(((Il2CppFullySharedGenericAny)(Il2CppFullySharedGenericAny*)L_3))), L_4);
		fsData_t6E144EC58C6E449805AB7BF77BCDAB7230BBFA23** L_6 = ___1_serialized;
		Dictionary_2_t9D50BDE1AE9A04E8610682F97C4DB702D3A9B4D3* L_7 = V_0;
		fsData_t6E144EC58C6E449805AB7BF77BCDAB7230BBFA23* L_8 = (fsData_t6E144EC58C6E449805AB7BF77BCDAB7230BBFA23*)il2cpp_codegen_object_new(fsData_t6E144EC58C6E449805AB7BF77BCDAB7230BBFA23_il2cpp_TypeInfo_var);
		fsData__ctor_mCF4D73BFD9271596000ACC3E17988E3492236781(L_8, L_7, NULL);
		*((RuntimeObject**)L_6) = (RuntimeObject*)L_8;
		Il2CppCodeGenWriteBarrier((void**)(RuntimeObject**)L_6, (void*)(RuntimeObject*)L_8);
		return L_5;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974 fsDirectConverter_1_TryDeserialize_m68AEEE5C3FB70E702E364D7B0C537966E0B079CF_gshared (fsDirectConverter_1_tCBA7B5D48312C1775A21082D3D453731C902A8BD* __this, fsData_t6E144EC58C6E449805AB7BF77BCDAB7230BBFA23* ___0_data, RuntimeObject** ___1_instance, Type_t* ___2_storageType, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	const uint32_t SizeOf_TModel_tA846BAB14AD72C3F97B08FAFEF7B2629C8054C13 = il2cpp_codegen_sizeof(il2cpp_rgctx_data_no_init(method->klass->rgctx_data, 1));
	const Il2CppFullySharedGenericAny L_10 = alloca(SizeOf_TModel_tA846BAB14AD72C3F97B08FAFEF7B2629C8054C13);
	const Il2CppFullySharedGenericAny L_18 = alloca(SizeOf_TModel_tA846BAB14AD72C3F97B08FAFEF7B2629C8054C13);
	fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974 V_0;
	memset((&V_0), 0, sizeof(V_0));
	Il2CppFullySharedGenericAny V_1 = alloca(SizeOf_TModel_tA846BAB14AD72C3F97B08FAFEF7B2629C8054C13);
	memset(V_1, 0, SizeOf_TModel_tA846BAB14AD72C3F97B08FAFEF7B2629C8054C13);
	fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974 V_2;
	memset((&V_2), 0, sizeof(V_2));
	{
		il2cpp_codegen_runtime_class_init_inline(fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974_il2cpp_TypeInfo_var);
		fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974 L_0 = ((fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974_StaticFields*)il2cpp_codegen_static_fields_for(fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974_il2cpp_TypeInfo_var))->___Success;
		V_0 = L_0;
		fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974 L_1 = V_0;
		fsData_t6E144EC58C6E449805AB7BF77BCDAB7230BBFA23* L_2 = ___0_data;
		NullCheck((fsBaseConverter_tDF9366828A5B8ADB6D986EB35B320D4DDC81CB04*)__this);
		fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974 L_3;
		L_3 = fsBaseConverter_CheckType_m14C4D0CAF424DF9F14D6365B5E5681C43FA82F32((fsBaseConverter_tDF9366828A5B8ADB6D986EB35B320D4DDC81CB04*)__this, L_2, (int32_t)1, NULL);
		fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974 L_4;
		L_4 = fsResult_op_Addition_mA94A4AD68668E539DEFE1255DC72B9D11A6DE41C(L_1, L_3, NULL);
		fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974 L_5 = L_4;
		V_0 = L_5;
		V_2 = L_5;
		bool L_6;
		L_6 = fsResult_get_Failed_m1398C627A72E75F5C7F8DB1A7C14E5B3271FF1FB((&V_2), NULL);
		if (!L_6)
		{
			goto IL_0022;
		}
	}
	{
		fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974 L_7 = V_0;
		return L_7;
	}

IL_0022:
	{
		RuntimeObject** L_8 = ___1_instance;
		RuntimeObject* L_9 = *((RuntimeObject**)L_8);
		void* L_11 = UnBox_Any(L_9, il2cpp_rgctx_data(method->klass->rgctx_data, 1), L_10);
		il2cpp_codegen_memcpy(V_1, (((Il2CppFullySharedGenericAny)(Il2CppFullySharedGenericAny*)L_11)), SizeOf_TModel_tA846BAB14AD72C3F97B08FAFEF7B2629C8054C13);
		fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974 L_12 = V_0;
		fsData_t6E144EC58C6E449805AB7BF77BCDAB7230BBFA23* L_13 = ___0_data;
		NullCheck(L_13);
		Dictionary_2_t9D50BDE1AE9A04E8610682F97C4DB702D3A9B4D3* L_14;
		L_14 = fsData_get_AsDictionary_m51779E71BBC994A7F5036BFB43F61B28C0817D86(L_13, NULL);
		fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974 L_15;
		L_15 = VirtualFuncInvoker2< fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974, Dictionary_2_t9D50BDE1AE9A04E8610682F97C4DB702D3A9B4D3*, Il2CppFullySharedGenericAny* >::Invoke(11, __this, L_14, (Il2CppFullySharedGenericAny*)V_1);
		il2cpp_codegen_runtime_class_init_inline(fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974_il2cpp_TypeInfo_var);
		fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974 L_16;
		L_16 = fsResult_op_Addition_mA94A4AD68668E539DEFE1255DC72B9D11A6DE41C(L_12, L_15, NULL);
		V_0 = L_16;
		RuntimeObject** L_17 = ___1_instance;
		il2cpp_codegen_memcpy(L_18, V_1, SizeOf_TModel_tA846BAB14AD72C3F97B08FAFEF7B2629C8054C13);
		RuntimeObject* L_19 = Box(il2cpp_rgctx_data_no_init(method->klass->rgctx_data, 1), L_18);
		*((RuntimeObject**)L_17) = (RuntimeObject*)L_19;
		Il2CppCodeGenWriteBarrier((void**)(RuntimeObject**)L_17, (void*)(RuntimeObject*)L_19);
		fsResult_tEB0578624BDC55E2B1775451433E6E3CBDA8D974 L_20 = V_0;
		return L_20;
	}
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void fsDirectConverter_1__ctor_mA219A5FF100DBCB35147606E9E4E9C565D2F8D50_gshared (fsDirectConverter_1_tCBA7B5D48312C1775A21082D3D453731C902A8BD* __this, const RuntimeMethod* method) 
{
	{
		fsDirectConverter__ctor_mCE6F7898DA2A3FBE101F5939D06A809E3164ABE1((fsDirectConverter_t9EE506A371C95C62E21CA6586396D7B3BD55F668*)__this, NULL);
		return;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool fsOption_1_get_HasValue_m2E9021BB4CB6156A14918A5B865D21751CA6D00C_gshared (fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88* __this, const RuntimeMethod* method) 
{
	{
		bool L_0 = *(bool*)il2cpp_codegen_get_instance_field_data_pointer(__this, il2cpp_rgctx_field(il2cpp_rgctx_data_no_init(InitializedTypeInfo(method->klass)->rgctx_data, 0),0));
		return L_0;
	}
}
IL2CPP_EXTERN_C  bool fsOption_1_get_HasValue_m2E9021BB4CB6156A14918A5B865D21751CA6D00C_AdjustorThunk (RuntimeObject* __this, const RuntimeMethod* method)
{
	fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88* _thisAdjusted;
	int32_t _offset = 1;
	_thisAdjusted = reinterpret_cast<fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88*>(__this + _offset);
	bool _returnValue;
	_returnValue = fsOption_1_get_HasValue_m2E9021BB4CB6156A14918A5B865D21751CA6D00C_inline(_thisAdjusted, method);
	return _returnValue;
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool fsOption_1_get_IsEmpty_m8F02E0608720E562CDB4C8906C1CD863FD8FE767_gshared (fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88* __this, const RuntimeMethod* method) 
{
	{
		bool L_0 = *(bool*)il2cpp_codegen_get_instance_field_data_pointer(__this, il2cpp_rgctx_field(il2cpp_rgctx_data_no_init(InitializedTypeInfo(method->klass)->rgctx_data, 0),0));
		return (bool)((((int32_t)L_0) == ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool fsOption_1_get_IsEmpty_m8F02E0608720E562CDB4C8906C1CD863FD8FE767_AdjustorThunk (RuntimeObject* __this, const RuntimeMethod* method)
{
	fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88* _thisAdjusted;
	int32_t _offset = 1;
	_thisAdjusted = reinterpret_cast<fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88*>(__this + _offset);
	bool _returnValue;
	_returnValue = fsOption_1_get_IsEmpty_m8F02E0608720E562CDB4C8906C1CD863FD8FE767(_thisAdjusted, method);
	return _returnValue;
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void fsOption_1_get_Value_m2A0C3FDC3098EA4DC3BB5CF398E95EC2C991988A_gshared (fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88* __this, Il2CppFullySharedGenericAny* il2cppRetVal, const RuntimeMethod* method) 
{
	const uint32_t SizeOf_T_tC5D84CBABB6B1934F21007E0A382E362C744EBA6 = il2cpp_codegen_sizeof(il2cpp_rgctx_data_no_init(InitializedTypeInfo(method->klass)->rgctx_data, 3));
	const Il2CppFullySharedGenericAny L_2 = alloca(SizeOf_T_tC5D84CBABB6B1934F21007E0A382E362C744EBA6);
	{
		bool L_0;
		L_0 = ((  bool (*) (fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88*, const RuntimeMethod*))il2cpp_codegen_get_direct_method_pointer(il2cpp_rgctx_method(InitializedTypeInfo(method->klass)->rgctx_data, 1)))(__this, il2cpp_rgctx_method(InitializedTypeInfo(method->klass)->rgctx_data, 1));
		if (!L_0)
		{
			goto IL_0013;
		}
	}
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_1 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_1, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral0A7D3B18DF4F9238A50A156FF06A5A7E794C1C7F)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_1, method);
	}

IL_0013:
	{
		il2cpp_codegen_memcpy(L_2, il2cpp_codegen_get_instance_field_data_pointer(__this, il2cpp_rgctx_field(il2cpp_rgctx_data_no_init(InitializedTypeInfo(method->klass)->rgctx_data, 0),1)), SizeOf_T_tC5D84CBABB6B1934F21007E0A382E362C744EBA6);
		il2cpp_codegen_memcpy(il2cppRetVal, L_2, SizeOf_T_tC5D84CBABB6B1934F21007E0A382E362C744EBA6);
		return;
	}
}
IL2CPP_EXTERN_C  void fsOption_1_get_Value_m2A0C3FDC3098EA4DC3BB5CF398E95EC2C991988A_AdjustorThunk (RuntimeObject* __this, Il2CppFullySharedGenericAny* il2cppRetVal, const RuntimeMethod* method)
{
	fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88* _thisAdjusted;
	int32_t _offset = 1;
	_thisAdjusted = reinterpret_cast<fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88*>(__this + _offset);
	fsOption_1_get_Value_m2A0C3FDC3098EA4DC3BB5CF398E95EC2C991988A(_thisAdjusted, il2cppRetVal, method);
	return;
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void fsOption_1__ctor_m343A7CF35E302DB9E5BCA27A0DAC03751B238516_gshared (fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88* __this, Il2CppFullySharedGenericAny ___0_value, const RuntimeMethod* method) 
{
	const uint32_t SizeOf_T_tC5D84CBABB6B1934F21007E0A382E362C744EBA6 = il2cpp_codegen_sizeof(il2cpp_rgctx_data_no_init(InitializedTypeInfo(method->klass)->rgctx_data, 3));
	const Il2CppFullySharedGenericAny L_0 = alloca(SizeOf_T_tC5D84CBABB6B1934F21007E0A382E362C744EBA6);
	{
		il2cpp_codegen_write_instance_field_data<bool>(__this, il2cpp_rgctx_field(il2cpp_rgctx_data_no_init(InitializedTypeInfo(method->klass)->rgctx_data, 0),0), (bool)1);
		il2cpp_codegen_memcpy(L_0, (il2cpp_codegen_class_is_value_type(il2cpp_rgctx_data_no_init(InitializedTypeInfo(method->klass)->rgctx_data, 3)) ? ___0_value : &___0_value), SizeOf_T_tC5D84CBABB6B1934F21007E0A382E362C744EBA6);
		il2cpp_codegen_write_instance_field_data(__this, il2cpp_rgctx_field(il2cpp_rgctx_data_no_init(InitializedTypeInfo(method->klass)->rgctx_data, 0),1), L_0, SizeOf_T_tC5D84CBABB6B1934F21007E0A382E362C744EBA6);
		return;
	}
}
IL2CPP_EXTERN_C  void fsOption_1__ctor_m343A7CF35E302DB9E5BCA27A0DAC03751B238516_AdjustorThunk (RuntimeObject* __this, Il2CppFullySharedGenericAny ___0_value, const RuntimeMethod* method)
{
	fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88* _thisAdjusted;
	int32_t _offset = 1;
	_thisAdjusted = reinterpret_cast<fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88*>(__this + _offset);
	fsOption_1__ctor_m343A7CF35E302DB9E5BCA27A0DAC03751B238516(_thisAdjusted, ___0_value, method);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool fsOption_1_get_HasValue_mA83E56B4674C678E7C760EABB73397EF37356E6F_gshared (fsOption_1_t5307CE32C845EE97F1173CDBB2385C4B81D2A28A* __this, const RuntimeMethod* method) 
{
	{
		bool L_0 = __this->____hasValue;
		return L_0;
	}
}
IL2CPP_EXTERN_C  bool fsOption_1_get_HasValue_mA83E56B4674C678E7C760EABB73397EF37356E6F_AdjustorThunk (RuntimeObject* __this, const RuntimeMethod* method)
{
	fsOption_1_t5307CE32C845EE97F1173CDBB2385C4B81D2A28A* _thisAdjusted;
	int32_t _offset = 1;
	_thisAdjusted = reinterpret_cast<fsOption_1_t5307CE32C845EE97F1173CDBB2385C4B81D2A28A*>(__this + _offset);
	bool _returnValue;
	_returnValue = fsOption_1_get_HasValue_mA83E56B4674C678E7C760EABB73397EF37356E6F_inline(_thisAdjusted, method);
	return _returnValue;
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool fsOption_1_get_IsEmpty_m95C6343EC3B5E180C487DF2F28019391A5B6672E_gshared (fsOption_1_t5307CE32C845EE97F1173CDBB2385C4B81D2A28A* __this, const RuntimeMethod* method) 
{
	{
		bool L_0 = __this->____hasValue;
		return (bool)((((int32_t)L_0) == ((int32_t)0))? 1 : 0);
	}
}
IL2CPP_EXTERN_C  bool fsOption_1_get_IsEmpty_m95C6343EC3B5E180C487DF2F28019391A5B6672E_AdjustorThunk (RuntimeObject* __this, const RuntimeMethod* method)
{
	fsOption_1_t5307CE32C845EE97F1173CDBB2385C4B81D2A28A* _thisAdjusted;
	int32_t _offset = 1;
	_thisAdjusted = reinterpret_cast<fsOption_1_t5307CE32C845EE97F1173CDBB2385C4B81D2A28A*>(__this + _offset);
	bool _returnValue;
	_returnValue = fsOption_1_get_IsEmpty_m95C6343EC3B5E180C487DF2F28019391A5B6672E(_thisAdjusted, method);
	return _returnValue;
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR fsVersionedType_tE569BD045F7DEDEC2122CC6CC0023B62B95332E8 fsOption_1_get_Value_m06DFFCCEB8E41DDEAEC89C3133DC3E75E3EA2241_gshared (fsOption_1_t5307CE32C845EE97F1173CDBB2385C4B81D2A28A* __this, const RuntimeMethod* method) 
{
	{
		bool L_0;
		L_0 = fsOption_1_get_IsEmpty_m95C6343EC3B5E180C487DF2F28019391A5B6672E(__this, il2cpp_rgctx_method(InitializedTypeInfo(method->klass)->rgctx_data, 1));
		if (!L_0)
		{
			goto IL_0013;
		}
	}
	{
		InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB* L_1 = (InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&InvalidOperationException_t5DDE4D49B7405FAAB1E4576F4715A42A3FAD4BAB_il2cpp_TypeInfo_var)));
		InvalidOperationException__ctor_mE4CB6F4712AB6D99A2358FBAE2E052B3EE976162(L_1, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral0A7D3B18DF4F9238A50A156FF06A5A7E794C1C7F)), NULL);
		IL2CPP_RAISE_MANAGED_EXCEPTION(L_1, method);
	}

IL_0013:
	{
		fsVersionedType_tE569BD045F7DEDEC2122CC6CC0023B62B95332E8 L_2 = __this->____value;
		return L_2;
	}
}
IL2CPP_EXTERN_C  fsVersionedType_tE569BD045F7DEDEC2122CC6CC0023B62B95332E8 fsOption_1_get_Value_m06DFFCCEB8E41DDEAEC89C3133DC3E75E3EA2241_AdjustorThunk (RuntimeObject* __this, const RuntimeMethod* method)
{
	fsOption_1_t5307CE32C845EE97F1173CDBB2385C4B81D2A28A* _thisAdjusted;
	int32_t _offset = 1;
	_thisAdjusted = reinterpret_cast<fsOption_1_t5307CE32C845EE97F1173CDBB2385C4B81D2A28A*>(__this + _offset);
	fsVersionedType_tE569BD045F7DEDEC2122CC6CC0023B62B95332E8 _returnValue;
	_returnValue = fsOption_1_get_Value_m06DFFCCEB8E41DDEAEC89C3133DC3E75E3EA2241(_thisAdjusted, method);
	return _returnValue;
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void fsOption_1__ctor_m22E0CD4999637C5B1CCFAD980FD29F2FFE3963B6_gshared (fsOption_1_t5307CE32C845EE97F1173CDBB2385C4B81D2A28A* __this, fsVersionedType_tE569BD045F7DEDEC2122CC6CC0023B62B95332E8 ___0_value, const RuntimeMethod* method) 
{
	{
		__this->____hasValue = (bool)1;
		fsVersionedType_tE569BD045F7DEDEC2122CC6CC0023B62B95332E8 L_0 = ___0_value;
		__this->____value = L_0;
		Il2CppCodeGenWriteBarrier((void**)&(((&__this->____value))->___Ancestors), (void*)NULL);
		#if IL2CPP_ENABLE_STRICT_WRITE_BARRIERS
		Il2CppCodeGenWriteBarrier((void**)&(((&__this->____value))->___VersionString), (void*)NULL);
		#endif
		#if IL2CPP_ENABLE_STRICT_WRITE_BARRIERS
		Il2CppCodeGenWriteBarrier((void**)&(((&__this->____value))->___ModelType), (void*)NULL);
		#endif
		return;
	}
}
IL2CPP_EXTERN_C  void fsOption_1__ctor_m22E0CD4999637C5B1CCFAD980FD29F2FFE3963B6_AdjustorThunk (RuntimeObject* __this, fsVersionedType_tE569BD045F7DEDEC2122CC6CC0023B62B95332E8 ___0_value, const RuntimeMethod* method)
{
	fsOption_1_t5307CE32C845EE97F1173CDBB2385C4B81D2A28A* _thisAdjusted;
	int32_t _offset = 1;
	_thisAdjusted = reinterpret_cast<fsOption_1_t5307CE32C845EE97F1173CDBB2385C4B81D2A28A*>(__this + _offset);
	fsOption_1__ctor_m22E0CD4999637C5B1CCFAD980FD29F2FFE3963B6(_thisAdjusted, ___0_value, method);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR bool fsOption_1_get_HasValue_m2E9021BB4CB6156A14918A5B865D21751CA6D00C_gshared_inline (fsOption_1_t5311B4288B7772566C60259BB204CF5737742E88* __this, const RuntimeMethod* method) 
{
	{
		bool L_0 = *(bool*)il2cpp_codegen_get_instance_field_data_pointer(__this, il2cpp_rgctx_field(il2cpp_rgctx_data_no_init(InitializedTypeInfo(method->klass)->rgctx_data, 0),0));
		return L_0;
	}
}
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR bool fsOption_1_get_HasValue_mA83E56B4674C678E7C760EABB73397EF37356E6F_gshared_inline (fsOption_1_t5307CE32C845EE97F1173CDBB2385C4B81D2A28A* __this, const RuntimeMethod* method) 
{
	{
		bool L_0 = __this->____hasValue;
		return L_0;
	}
}
